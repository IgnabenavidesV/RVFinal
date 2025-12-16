using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class IceTower : MonoBehaviour
{
    public float range = 8f;
    public float fireRate = 1f;
    public float rotationSpeed = 5f;

    public Transform head;
    public Transform shootPoint;
    public GameObject iceProjectilePrefab;
    public AudioClip shootAudioClip;
    private AudioSource audioSource;

    private float fireTimer = 0f;
    private MonoBehaviour currentTarget;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
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
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        currentTarget = enemies
            .Select(e => e.GetComponent<Balloon>() as MonoBehaviour ?? e.GetComponent<Enemy>())
            .Where(e => e != null && Vector3.Distance(transform.position, e.transform.position) <= range)
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .FirstOrDefault();
    }

    void RotateToTarget()
    {
        Vector3 dir = currentTarget.transform.position - head.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);

        head.rotation = Quaternion.Lerp(head.rotation, lookRot, rotationSpeed * Time.deltaTime);

        if (shootPoint != null)
            shootPoint.rotation = lookRot;
    }

    void Shoot()
    {
        GameObject go = Instantiate(iceProjectilePrefab, shootPoint.position, shootPoint.rotation);
        IceProjectile p = go.GetComponent<IceProjectile>();
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
            b.TakeDamage(5);
            b.ApplySlow(0.5f, 2f);
            return;
        }

        Enemy e = enemy.GetComponent<Enemy>();
        if (e != null)
        {
            e.TakeDamage(5);
            e.ApplySlow(0.5f, 2f);
        }
    }
}
