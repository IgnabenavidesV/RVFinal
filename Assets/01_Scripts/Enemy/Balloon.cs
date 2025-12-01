using UnityEngine;
using System.Collections;

public class Balloon : MonoBehaviour
{
    public float speed = 1.5f;
    public int life = 1;

    [HideInInspector] public Transform[] path;
    private int currentPoint = 0;

    public float floatAmplitude = 0.02f;
    public float floatFrequency = 1f;
    public float rotationSpeed = 20f;

    private Vector3 startPos;

    [HideInInspector] public WaveManager waveManager;

    // --- SISTEMA DE SLOW ---
    private float originalSpeed;
    private float slowTimer = 0f;
    private bool isStunned = false;
    private bool isBurning = false;
    private bool isPoisoned = false;

    void Start()
    {
        originalSpeed = speed;
        startPos = transform.position;

        Transform wp = GameObject.Find("Waypoints").transform;

        path = new Transform[wp.childCount];
        for (int i = 0; i < wp.childCount; i++)
        {
            path[i] = wp.GetChild(i);
        }
    }

    void Update()
    {
        // Contador del slow
        if (slowTimer > 0)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0)
            {
                speed = originalSpeed; // Se recupera
            }
        }

        if (path == null || path.Length == 0) return;

        Transform target = path[currentPoint];

        // Movimiento base
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // Flotación
        float yOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);

        // Rotación
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Llegó al waypoint
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentPoint++;

            if (currentPoint >= path.Length)
            {
                if (waveManager != null)
                    waveManager.OnEnemyKilled();

                Destroy(gameObject);
            }
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
        float originalSpeed = speed;
        speed = 0;   // Detener movimiento

        yield return new WaitForSeconds(duration);

        speed = originalSpeed;
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
        float tick = 0.5f;   // daño cada medio segundo

        while (timer < duration)
        {
            TakeDamage(Mathf.RoundToInt(dps * tick)); // daño proporcional
            timer += tick;
            yield return new WaitForSeconds(tick);
        }

        isBurning = false;
    }
    public void ApplyPoison(float duration, float dps)
    {
        // Si ya está envenenado, reinicia el veneno
        StartCoroutine(PoisonRoutine(duration, dps));
    }

    private IEnumerator PoisonRoutine(float duration, float dps)
    {
        isPoisoned = true;

        float timer = 0f;
        float tick = 1f; // daño cada segundo

        while (timer < duration)
        {
            TakeDamage(Mathf.RoundToInt(dps * tick));
            timer += tick;
            yield return new WaitForSeconds(tick);
        }

        isPoisoned = false;
    }
    public void TakeDamage(int dmg)
    {
        life -= dmg;
        if (life <= 0)
        {
            if (waveManager != null)
                waveManager.OnEnemyKilled();

            Destroy(gameObject);
        }
    }

}
