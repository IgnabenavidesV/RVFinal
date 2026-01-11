using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ArrowTower : MonoBehaviour
{
    [Header("Tower Settings")]
    public float range = 8f;
    public float fireRate = 1f;
    public float rotationSpeed = 5f;

    [Header("Damage (Inspector)")]
    public int damage = 10;

    [Header("References")]
    public Transform head;
    public Transform shootPoint;
    public GameObject arrowPrefab;
    public AudioClip shootAudioClip;
    private AudioSource audioSource;

    private float fireCooldown;
    private List<MonoBehaviour> enemiesInRange = new List<MonoBehaviour>();
    private MonoBehaviour currentTarget;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        SelectTarget();
        if (currentTarget != null) AimAtTarget();

        if (currentTarget != null && fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = 1f / fireRate;
        }

        fireCooldown -= Time.deltaTime;
    }

    void SelectTarget()
    {
        float shortestDist = Mathf.Infinity;
        MonoBehaviour nearest = null;

        enemiesInRange = enemiesInRange
            .Where(e => e != null && Vector3.Distance(transform.position, e.transform.position) <= range)
            .ToList();

        foreach (var enemy in enemiesInRange)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < shortestDist)
            {
                shortestDist = dist;
                nearest = enemy;
            }
        }

        currentTarget = nearest;
    }

    void AimAtTarget()
    {
        if (currentTarget == null || head == null) return;

        Vector3 dir = currentTarget.transform.position - head.position;
        dir.y = 0;

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            head.rotation = Quaternion.Lerp(head.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        if (shootPoint != null)
            shootPoint.LookAt(currentTarget.transform.position);
    }

    void Shoot()
    {
        GameObject arrowObj = Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);

        ArrowProjectile proj = arrowObj.GetComponent<ArrowProjectile>();
        if (proj != null)
            proj.SetTarget(currentTarget.transform);

        // ✅ DAÑO desde Inspector
        ApplyDamage(currentTarget);

        if (shootAudioClip != null)
            audioSource.PlayOneShot(shootAudioClip);
    }

    void ApplyDamage(MonoBehaviour enemy)
    {
        Balloon b = enemy.GetComponent<Balloon>();
        if (b != null)
        {
            b.TakeDamage(damage);
            return;
        }

        Enemy e = enemy.GetComponent<Enemy>();
        if (e != null)
        {
            e.TakeDamage(damage);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        MonoBehaviour enemy = col.GetComponent<Balloon>() as MonoBehaviour ?? col.GetComponent<Enemy>();
        if (enemy != null && !enemiesInRange.Contains(enemy))
            enemiesInRange.Add(enemy);
    }

    private void OnTriggerExit(Collider col)
    {
        MonoBehaviour enemy = col.GetComponent<Balloon>() as MonoBehaviour ?? col.GetComponent<Enemy>();
        if (enemy != null)
            enemiesInRange.Remove(enemy);
    }
}
