using UnityEngine;

public class Balloon : MonoBehaviour
{
    public float speed = 1.5f;
    public int life = 1;

    [HideInInspector] public Transform[] path;
    private int currentPoint = 0;

    // Efecto de flotación
    public float floatAmplitude = 0.02f;
    public float floatFrequency = 1f;
    public float rotationSpeed = 20f;

    private Vector3 startPos;

    // Referencia al WaveManager
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

        // Movimiento hacia el siguiente punto
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // Movimiento vertical sutil
        float yOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);

        // Rotación suave
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Comprobar si llegó al waypoint
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentPoint++;

            // Si ya pasó todos los puntos
            if (currentPoint >= path.Length)
            {
                // Aquí puedes restar vida al jugador si lo usas
                // GameManager.Instance.LoseLife();

                // Avisar al WaveManager que este globo murió
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
        waveManager.OnEnemyKilled();
        Destroy(gameObject);
    }

}
