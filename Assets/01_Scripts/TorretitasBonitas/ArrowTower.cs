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

    private float fireCooldown;
    private List<Transform> enemiesInRange = new List<Transform>();
    private Transform currentTarget;

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

    // 🔎 Selecciona el enemigo más cercano
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

    // 🎯 Rota la cabeza hacia el objetivo
    void AimAtTarget()
    {
        Vector3 dir = currentTarget.position - head.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        head.rotation = Quaternion.Lerp(head.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    // 🏹 Disparo de flecha
    void Shoot()
    {
        GameObject arrowObj = Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
        arrowObj.GetComponent<ArrowProjectile>().SetTarget(currentTarget);

    }

    // ⚠️ ENTRADA de enemigos al rango
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Enemy"))
            enemiesInRange.Add(col.transform);
    }

    // ❌ SALIDA de enemigos del rango
    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Enemy"))
            enemiesInRange.Remove(col.transform);
    }
}
