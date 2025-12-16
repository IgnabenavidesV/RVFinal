using UnityEngine;
using System.Linq;

public class PoisonTower : MonoBehaviour
{
    [Header("Tower Stats")]
    public float range = 8f;
    public float fireRate = 1.2f;
    public float rotationSpeed = 5f;

    [Header("References")]
    public Transform head;
    public Transform shootPoint;
    public GameObject poisonProjectilePrefab;

    [Header("Audio")]
    public AudioClip shootClip;
    private AudioSource audioSource;

    private float fireTimer = 0f;
    private Transform currentTarget;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        if (shootPoint == null) shootPoint = head;
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;

        FindTarget();

        if (currentTarget != null)
        {
            RotateHead();
            AimShootPoint();

            if (fireTimer <= 0f)
            {
                Shoot();
                fireTimer = 1f / fireRate;
            }
        }
    }

    void FindTarget()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        currentTarget = enemies
            .Where(e => Vector3.Distance(transform.position, e.transform.position) <= range)
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .Select(e => e.transform)
            .FirstOrDefault();
    }

    void RotateHead()
    {
        if (currentTarget == null || head == null)
            return;

        Vector3 dir = currentTarget.position - head.position;
        dir.y = 0; // solo horizontal

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            head.rotation = Quaternion.Lerp(head.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    void AimShootPoint()
    {
        if (shootPoint != null && currentTarget != null)
            shootPoint.LookAt(currentTarget.position); // apuntar al objetivo
    }

    void Shoot()
    {
        if (poisonProjectilePrefab == null || shootPoint == null) return;

        GameObject proj = Instantiate(poisonProjectilePrefab, shootPoint.position, shootPoint.rotation);

        PoisonProjectile p = proj.GetComponent<PoisonProjectile>();
        if (p != null)
            p.SetTarget(currentTarget);

        if (shootClip != null)
            audioSource.PlayOneShot(shootClip);
    }
}
