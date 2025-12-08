using UnityEngine;

public class TestTurret : MonoBehaviour
{
    [Header("Stats de torre")]
    public float range = 10f;
    public float fireRate = 1f;
    public int damagePerShot = 1;      // 🔥 ÚNICO lugar donde seteas el daño

    [Header("Disparo")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Rotación")]
    public float rotationSpeed = 5f;

    private float fireCountdown;
    private Transform currentTarget;

    private void Update()
    {
        FindTarget();

        if (currentTarget == null) return;

        RotateTowardsTarget();

        fireCountdown -= Time.deltaTime;

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }
    }

    private void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        float shortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = hit.transform;
                }
            }
        }

        currentTarget = nearestEnemy;
    }

    private void RotateTowardsTarget()
    {
        Vector3 dir = currentTarget.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        Vector3 dir = (currentTarget.position - firePoint.position).normalized;
        firePoint.rotation = Quaternion.LookRotation(dir);

        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Bullet bullet = bulletGO.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.damage = damagePerShot; // aquí se pasa el daño
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
