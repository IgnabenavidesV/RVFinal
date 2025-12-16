using UnityEngine;

public class BlimpEnemy : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 2f;

    private Transform[] path;
    private int currentPoint = 0;

    public float floatAmplitude = 0.02f;
    public float floatFrequency = 1f;
    public float rotationSpeed = 20f;

    private Vector3 startPos;

    [Header("Ataque")]
    public float detectionRange = 10f;
    public float fireRate = 1f;
    public int damage = 10;
    public GameObject bulletPrefab;
    public Transform shootPoint;

    private float fireCooldown = 0f;
    private Transform targetTower;

    void Start()
    {
        startPos = transform.position;

        // Igual que Balloon pero con WaypointsDirigible
        Transform wp = GameObject.Find("WaypointsDirigible").transform;

        path = new Transform[wp.childCount];
        for (int i = 0; i < wp.childCount; i++)
        {
            path[i] = wp.GetChild(i);
        }

        // Empieza en un punto random
        if (path.Length > 0)
            currentPoint = Random.Range(0, path.Length);
    }

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        DetectTower();

        // 🔥 Solo atacar si está realmente en rango,
        // si no, patrulla normalmente.
        if (targetTower != null && Vector3.Distance(transform.position, targetTower.position) <= detectionRange)
            AttackTower();
        else
            Patrol();
    }


    // ------------------ PATRULLA ----------------------
    void Patrol()
    {
        if (path == null || path.Length == 0) return;

        Transform target = path[currentPoint];

        // Movimiento base
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // Flotación MUY leve
        float yOffset = Mathf.Sin(Time.time * floatFrequency) * (floatAmplitude * 0.2f);
        transform.position += new Vector3(0, yOffset, 0);

        // Rotación suave
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Llegó al punto → elige otro random
        if (Vector3.Distance(transform.position, target.position) < 0.15f)
        {
            currentPoint = Random.Range(0, path.Length);
        }
    }

    // ------------------ DETECTAR TORRETAS ----------------------
    void DetectTower()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange);

        targetTower = null;
        float closest = Mathf.Infinity;

        foreach (var h in hits)
        {
            // ✅ En vez de usar tag, buscamos TowerHealth en el PARENT
            TowerHealth th = h.GetComponentInParent<TowerHealth>();
            if (th != null)
            {
                float dist = Vector3.Distance(transform.position, th.transform.position);
                if (dist < closest)
                {
                    closest = dist;
                    targetTower = th.transform;
                }
            }
        }
    }

    // ------------------ ATACAR TORRETA ----------------------
    void AttackTower()
    {
        if (targetTower == null) return;

        // Mirar pero no modificar Y (evita rotaciones raras)
        Vector3 lookPos = new Vector3(targetTower.position.x, transform.position.y, targetTower.position.z);
        transform.LookAt(lookPos);

        // 🔥 Disparar
        if (fireCooldown <= 0f)
        {
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);

            BlimpBullet bb = bullet.GetComponent<BlimpBullet>();
            if (bb != null) bb.damage = damage;

            fireCooldown = fireRate;
            Debug.Log("💥 Dirigible disparó a " + targetTower.name);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
