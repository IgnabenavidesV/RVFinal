using UnityEngine;
using System.Linq;

public class FireTower : MonoBehaviour
{
    [Header("Tower Stats")]
    public float range = 8f;
    public float fireRate = 1f;
    public float rotationSpeed = 5f; // ? igual que PoisonTower

    [Header("References")]
    public Transform head;
    public Transform shootPoint;
    public GameObject fireProjectilePrefab;

    [Header("Audio")]
    public AudioClip shootAudioClip;
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
        if (currentTarget == null || head == null) return;

        // ? solo horizontal
        Vector3 dir = currentTarget.position - head.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            head.rotation = Quaternion.Lerp(head.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    void AimShootPoint()
    {
        // ? shootPoint apunta al enemigo con altura
        if (shootPoint != null && currentTarget != null)
            shootPoint.LookAt(currentTarget.position);
    }

    void Shoot()
    {
        if (fireProjectilePrefab == null || shootPoint == null) return;

        GameObject proj = Instantiate(fireProjectilePrefab, shootPoint.position, shootPoint.rotation);

        FireProjectile fp = proj.GetComponent<FireProjectile>();
        if (fp != null)
            fp.SetTarget(currentTarget);

        if (shootAudioClip != null)
            audioSource.PlayOneShot(shootAudioClip);
    }
}
