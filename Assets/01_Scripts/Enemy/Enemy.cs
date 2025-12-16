using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats (Inspector)")]
    [Min(1)] public int health = 3;
    public int damageToPlayer = 1;
    public float moveSpeed = 2f;

    [Header("Waypoint Movement")]
    public Transform[] path;
    private int currentPoint = 0;

    [Header("Floating Animation")]
    public float floatAmplitudeY = 0.5f;
    public float floatFrequencyY = 0.8f;
    public float floatAmplitudeX = 0.2f;
    public float floatFrequencyX = 0.5f;
    public float rotationSpeed = 20f;

    [Header("Audio")]
    public AudioClip deathClip;
    public AudioClip reachGoalClip;

    [Header("Economía")]
    public int moneyReward = 5; // Editable en el inspector

    [HideInInspector] public WaveManager waveManager;

    private AudioSource audioSource;
    private bool isDead = false;

    // Efectos
    private float originalSpeed;
    private float slowTimer = 0f;
    private bool isStunned = false;
    private bool isBurning = false;
    private bool isPoisoned = false;

    private void Awake()
    {
        originalSpeed = moveSpeed;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        // Tag del enemigo
        gameObject.tag = "Enemy";
    }

    private void Update()
    {
        if (isDead) return;

        if (slowTimer > 0f)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0f) moveSpeed = originalSpeed;
        }

        if (path != null && path.Length > 0 && !isStunned)
            MoveAlongPath();

        AnimateFloating();
    }

    private void MoveAlongPath()
    {
        Transform target = path[currentPoint];
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        Vector3 dir = (target.position - transform.position).normalized;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 2f);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentPoint++;
            if (currentPoint >= path.Length)
                ReachGoal();
        }
    }

    private void AnimateFloating()
    {
        float time = Time.time;
        float yOffset = Mathf.Sin(time * floatFrequencyY) * floatAmplitudeY;
        float xOffset = Mathf.Sin(time * floatFrequencyX) * floatAmplitudeX;

        transform.position += new Vector3(xOffset, yOffset, 0f) * Time.deltaTime;

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.forward, rotationSpeed / 2f * Time.deltaTime, Space.World);
    }

    // ====== DAÑO / EFECTOS ======
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        health -= amount;

        if (health <= 0) Die();
    }

    public void ApplySlow(float percent, float duration)
    {
        moveSpeed = originalSpeed * (1f - percent);
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
        float saved = moveSpeed;
        moveSpeed = 0f;

        yield return new WaitForSeconds(duration);

        moveSpeed = saved;
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

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        waveManager?.OnEnemyKilled();

        // Dar dinero al jugador
        GameManager.Instance.AddMoney(moneyReward);

        if (deathClip != null) audioSource.PlayOneShot(deathClip);
        Destroy(gameObject, deathClip != null ? deathClip.length : 0f);
    }

    private void ReachGoal()
    {
        if (isDead) return;
        isDead = true;

        // ✅ DAÑO AL PLAYER
        if (GameManager.Instance != null)
            GameManager.Instance.TakeDamage(damageToPlayer);

        waveManager?.OnEnemyKilled();

        if (reachGoalClip != null)
            audioSource.PlayOneShot(reachGoalClip);

        Destroy(gameObject, reachGoalClip != null ? reachGoalClip.length : 0f);
    }


    public void SetPath(Transform[] waypoints)
    {
        path = waypoints;
        currentPoint = 0;
    }
}
