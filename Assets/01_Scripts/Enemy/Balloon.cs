using UnityEngine;

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

    void Start()
    {
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

    void Die()
    {
        if (waveManager != null)
            waveManager.OnEnemyKilled();

        Destroy(gameObject);
    }
}
