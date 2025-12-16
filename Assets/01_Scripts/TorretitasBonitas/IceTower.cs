using UnityEngine;
using System.Linq;

public class IceTower : MonoBehaviour
{
    public float range = 8f;
    public float fireRate = 1f;
    public float rotationSpeed = 5f;

    public Transform head;
    public Transform shootPoint;
    public GameObject iceProjectilePrefab;

    [Header("Audio")]
    public AudioClip shootClip;
    private AudioSource audioSource;

    private float fireTimer = 0f;
    private Transform currentTarget;

    void Awake()
    {
        // Detecta automáticamente o crea AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;

        FindTarget();

        if (currentTarget != null)
        {
            RotateToTarget();

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

        var closest = enemies
            .Where(e => Vector3.Distance(transform.position, e.transform.position) <= range)
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .FirstOrDefault();

        currentTarget = closest != null ? closest.transform : null;
    }

    void RotateToTarget()
    {
        Vector3 dir = currentTarget.position - head.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        head.rotation = Quaternion.Lerp(head.rotation, lookRot, rotationSpeed * Time.deltaTime);

        if (shootPoint != null)
            shootPoint.rotation = lookRot;
    }

    void Shoot()
    {
        // Disparo
        GameObject go = Instantiate(iceProjectilePrefab, shootPoint.position, shootPoint.rotation);
        var p = go.GetComponent<IceProjectile>();
        p.SetTarget(currentTarget);

        // Audio
        if (shootClip != null)
            audioSource.PlayOneShot(shootClip);
    }
}
