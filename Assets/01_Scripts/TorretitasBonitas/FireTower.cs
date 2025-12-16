using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class FireTower : MonoBehaviour
{
    public float range = 8f;
    public float fireRate = 1f;

    public Transform head;
    public Transform shootPoint;
    public GameObject fireProjectilePrefab;
    public AudioClip shootAudioClip;
    private AudioSource audioSource;

    private float cooldown = 0f;
    private MonoBehaviour currentTarget; // Balloon o Enemy

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        cooldown -= Time.deltaTime;

        FindTarget();

        if (currentTarget != null)
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

        currentTarget = enemies
            .Select(e => e.GetComponent<Balloon>() as MonoBehaviour ?? e.GetComponent<Enemy>())
            .Where(e => e != null && Vector3.Distance(transform.position, e.transform.position) <= range)
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .FirstOrDefault();
    }

    void RotateTowardsTarget()
    {
        Vector3 dir = currentTarget.transform.position - head.position;
        Quaternion rot = Quaternion.LookRotation(dir);
        head.rotation = Quaternion.Lerp(head.rotation, rot, Time.deltaTime * 5f);

        if (shootPoint != null)
            shootPoint.rotation = head.rotation;
    }

    void Shoot()
    {
        GameObject proj = Instantiate(fireProjectilePrefab, shootPoint.position, shootPoint.rotation);
        FireProjectile p = proj.GetComponent<FireProjectile>();
        p.SetTarget(currentTarget.transform);

        ApplyEffects(currentTarget);

        if (shootAudioClip != null)
            audioSource.PlayOneShot(shootAudioClip);
    }

    void ApplyEffects(MonoBehaviour enemy)
    {
        Balloon b = enemy.GetComponent<Balloon>();
        if (b != null)
        {
            b.TakeDamage(10);
            b.ApplyBurn(2f, 2f);
            return;
        }

        Enemy e = enemy.GetComponent<Enemy>();
        if (e != null)
        {
            e.TakeDamage(10);
            e.ApplyBurn(2f, 2f);
        }
    }
}
