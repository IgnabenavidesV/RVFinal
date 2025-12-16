using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ArrowTower : MonoBehaviour
{
    [Header("Tower Settings")]
    public float range = 8f;
    public float fireRate = 1f;
    public float rotationSpeed = 5f;

    [Header("References")]
    public Transform head;
    public Transform shootPoint;
    public GameObject arrowPrefab;
    public AudioClip shootAudioClip;
    private AudioSource audioSource;

    private float fireCooldown;
    private List<MonoBehaviour> enemiesInRange = new List<MonoBehaviour>(); // Balloon o Enemy
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

        // Filtrar solo enemigos dentro del rango
        enemiesInRange = enemiesInRange.Where(e => e != null && Vector3.Distance(transform.position, e.transform.position) <= range).ToList();

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
        Vector3 dir = currentTarget.transform.position - head.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        head.rotation = Quaternion.Lerp(head.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        if (shootPoint != null)
            shootPoint.rotation = lookRotation;
    }

    void Shoot()
    {
        GameObject arrowObj = Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
        ArrowProjectile proj = arrowObj.GetComponent<ArrowProjectile>();
        proj.SetTarget(currentTarget.transform);

        ApplyEffects(currentTarget);

        if (shootAudioClip != null)
            audioSource.PlayOneShot(shootAudioClip);
    }

    void ApplyEffects(MonoBehaviour enemy)
    {
        Balloon b = enemy.GetComponent<Balloon>();
        if (b != null)
        {
            b.TakeDamage(10); // ejemplo de daño
            // b.ApplyBurn(...); // puedes añadir efectos extra si quieres
            return;
        }

        Enemy e = enemy.GetComponent<Enemy>();
        if (e != null)
        {
            e.TakeDamage(10); // ejemplo de daño
            // e.ApplyBurn(...); // puedes añadir efectos extra si quieres
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
