using System.Collections.Generic;
using UnityEngine;

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
    public AudioClip shootAudioClip; // Audio al disparar
    private AudioSource audioSource;

    private float fireCooldown;
    private List<Transform> enemiesInRange = new List<Transform>();
    private Transform currentTarget;

    void Start()
    {
        // Configuramos AudioSource automáticamente
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        SelectTarget();
        if (currentTarget) AimAtTarget();

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
        Transform nearestEnemy = null;

        foreach (var enemy in enemiesInRange)
        {
            if (enemy == null) continue;

            float dist = Vector3.Distance(transform.position, enemy.position);
            if (dist < shortestDist)
            {
                shortestDist = dist;
                nearestEnemy = enemy;
            }
        }

        currentTarget = nearestEnemy;
    }

    void AimAtTarget()
    {
        Vector3 dir = currentTarget.position - head.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        head.rotation = Quaternion.Lerp(head.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        if (shootPoint != null)
        {
            shootPoint.rotation = lookRotation;
        }
    }

    void Shoot()
    {
        GameObject arrowObj = Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
        arrowObj.GetComponent<ArrowProjectile>().SetTarget(currentTarget);

        if (shootAudioClip != null)
            audioSource.PlayOneShot(shootAudioClip);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Enemy"))
            enemiesInRange.Add(col.transform);
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Enemy"))
            enemiesInRange.Remove(col.transform);
    }
}
