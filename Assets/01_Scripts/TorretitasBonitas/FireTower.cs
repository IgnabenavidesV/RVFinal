using UnityEngine;
using System.Linq;

public class FireTower : MonoBehaviour
{
    public float range = 8f;
    public float fireRate = 1f;

    public Transform head;
    public Transform shootPoint;
    public GameObject fireProjectilePrefab;
    public AudioClip shootAudioClip; // Audio al disparar
    private AudioSource audioSource;

    private float cooldown = 0f;
    private Balloon target;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        cooldown -= Time.deltaTime;

        FindTarget();

        if (target != null)
        {
            RotateTowardsTarget();

            if (cooldown <= 0f)
            {
                Shoot();
                cooldown = 1f / fireRate;
            }
        }
    }

    void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        target = enemies
            .Select(e => e.GetComponent<Balloon>())
            .Where(b => b != null && Vector3.Distance(transform.position, b.transform.position) <= range)
            .OrderBy(b => Vector3.Distance(transform.position, b.transform.position))
            .FirstOrDefault();
    }

    void RotateTowardsTarget()
    {
        Vector3 dir = target.transform.position - head.position;
        Quaternion rot = Quaternion.LookRotation(dir);
        head.rotation = Quaternion.Lerp(head.rotation, rot, Time.deltaTime * 5f);

        if (shootPoint != null)
            shootPoint.rotation = head.rotation;
    }

    void Shoot()
    {
        GameObject proj = Instantiate(fireProjectilePrefab, shootPoint.position, shootPoint.rotation);
        FireProjectile p = proj.GetComponent<FireProjectile>();
        p.SetTarget(target.transform);

        if (shootAudioClip != null)
            audioSource.PlayOneShot(shootAudioClip);
    }
}
