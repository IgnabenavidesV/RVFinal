using UnityEngine;
using System.Linq;

public class PoisonTower : MonoBehaviour
{
    public float range = 8f;
    public float fireRate = 1.2f;
    public Transform head;
    public Transform shootPoint;

    public GameObject poisonProjectilePrefab;

    private float cooldown = 0f;
    private Balloon target;

    void Update()
    {
        cooldown -= Time.deltaTime;

        FindTarget();

        if (target != null)
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

        target = enemies
            .Select(e => e.GetComponent<Balloon>())
            .Where(b => b != null && Vector3.Distance(transform.position, b.transform.position) <= range)
            .OrderBy(b => Vector3.Distance(transform.position, b.transform.position))
            .FirstOrDefault();
    }

    void RotateTowardsTarget()
    {
        Vector3 dir = target.transform.position - head.position;
        Quaternion rot = Quaternion.LookRotation(dir);
        head.rotation = Quaternion.Lerp(head.rotation, rot, Time.deltaTime * 5f);
    }

    void Shoot()
    {
        GameObject proj = Instantiate(poisonProjectilePrefab, shootPoint.position, shootPoint.rotation);

        PoisonProjectile p = proj.GetComponent<PoisonProjectile>();
        p.SetTarget(target.transform);
    }
}
