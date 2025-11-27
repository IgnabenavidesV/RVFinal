using UnityEngine;

public class Balloon : MonoBehaviour
{
    public float speed = 1.5f;
    public int life = 1;

    [HideInInspector] public Transform[] path;
    private int currentPoint = 0;

    // Variables para el efecto de globo aerostático
    public float floatAmplitude = 0.02f; // altura mínima, en unidades de Unity (~1-2 pixeles)
    public float floatFrequency = 1f;    // velocidad del vaivén
    public float rotationSpeed = 20f;    // grados por segundo

    private Vector3 startPos;

    void Start()
    {
        // Guardar posición inicial para el movimiento vertical
        startPos = transform.position;

        // Buscar automáticamente el objeto "Waypoints"
        Transform wp = GameObject.Find("Waypoints").transform;

        // Crear array con sus hijos (WP0, WP1, WP2...)
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

        // Movimiento vertical sutil (flotación)
        float yOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);

        // Rotación suave
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Comprobar si llegó al waypoint
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentPoint++;
            if (currentPoint >= path.Length)
            {
                Destroy(gameObject);
            }
        }
    }

    public void TakeDamage(int dmg)
    {
        life -= dmg;
        if (life <= 0)
        {
            Destroy(gameObject);
        }
    }
}
