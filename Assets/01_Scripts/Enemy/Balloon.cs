using System.Collections;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    [Header("Boss Stats (Inspector manda)")]
    [Min(1)] public int health = 30;
    public float speed = 1.5f;

    [HideInInspector] public Transform[] path;
    [HideInInspector] public WaveManager waveManager;

    private int currentPoint = 0;

    // efectos
    private float originalSpeed;
    private float slowTimer = 0f;
    private bool isStunned = false;
    private bool isBurning = false;
    private bool isPoisoned = false;

    private void Awake()
    {
        originalSpeed = speed;

        // si quieres que la torre lo encuentre por tag:
        gameObject.tag = "Enemy";
        // y si quieres distinguirlo:
        // gameObject.tag = "Boss";  (pero entonces tu torre debe buscar Boss también)
    }

    void Update()
    {
        if (slowTimer > 0f)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0f) speed = originalSpeed;
        }

        if (path == null || path.Length == 0) return;

        Transform target = path[currentPoint];

        if (!isStunned)
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentPoint++;
            if (currentPoint >= path.Length)
            {
                waveManager?.OnEnemyKilled();
                Destroy(gameObject);
            }
        }
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        Debug.Log($"[Boss] {name} HIT dmg={dmg} hp={health}");

        if (health <= 0)
        {
            waveManager?.OnEnemyKilled();
            Destroy(gameObject);
        }
    }

    public void ApplySlow(float percent, float duration)
    {
        speed = originalSpeed * (1f - percent);
        slowTimer = duration;
    }

    public void ApplyStun(float duration)
    {
        if (isStunned) return;
        StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    public void ApplyBurn(float duration, float dps)
    {
        if (isBurning) return;
        StartCoroutine(BurnRoutine(duration, dps));
    }

    private IEnumerator BurnRoutine(float duration, float dps)
    {
        isBurning = true;
        float timer = 0f;
        float tick = 0.5f;

        while (timer < duration)
        {
            TakeDamage(Mathf.RoundToInt(dps * tick));
            timer += tick;
            yield return new WaitForSeconds(tick);
        }

        isBurning = false;
    }

    public void ApplyPoison(float duration, float dps)
    {
        if (isPoisoned) StopCoroutine(nameof(PoisonRoutine));
        StartCoroutine(PoisonRoutine(duration, dps));
    }

    private IEnumerator PoisonRoutine(float duration, float dps)
    {
        isPoisoned = true;
        float timer = 0f;
        float tick = 1f;

        while (timer < duration)
        {
            TakeDamage(Mathf.RoundToInt(dps * tick));
            timer += tick;
            yield return new WaitForSeconds(tick);
        }

        isPoisoned = false;
    }
}
