using UnityEngine;
using System.Linq;

public class IceTower : MonoBehaviour
{
    [Header("Tower Stats")]
    public float range = 8f;
    public float fireRate = 1f;
    public float rotationSpeed = 5f;

    [Header("References")]
    public Transform head;          // Parte superior que rota horizontalmente
    public Transform shootPoint;    // Punto de disparo
    public GameObject iceProjectilePrefab;

    [Header("Audio")]
    public AudioClip shootClip;
    private AudioSource audioSource;

    private float fireTimer = 0f;
    private Transform currentTarget;

    void Awake()
    {
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

        // Dirección hacia el enemigo (solo horizontal)
        Vector3 dir = currentTarget.position - head.position;
        dir.y = 0; // Mantener solo rotación horizontal

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            head.rotation = Quaternion.Lerp(head.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }


void AimShootPoint()
{
    if (shootPoint != null && currentTarget != null)
    {
        // Apunta al enemigo incluyendo altura
        shootPoint.LookAt(currentTarget.position);
    }
}

void Shoot()
{
    if (iceProjectilePrefab == null || shootPoint == null)
        return;

    GameObject go = Instantiate(iceProjectilePrefab, shootPoint.position, shootPoint.rotation);
    var p = go.GetComponent<IceProjectile>();
    if (p != null)
        p.SetTarget(currentTarget);

    if (shootClip != null)
        audioSource.PlayOneShot(shootClip);
}
}
