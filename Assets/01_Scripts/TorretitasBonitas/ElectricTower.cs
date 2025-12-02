using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ElectricTower : MonoBehaviour
{
    public float range = 8f;
    public float chainRange = 4f;       // rango para la cadena
    public int maxChainTargets = 3;     // cuántos enemigos puede aturdir
    public float fireRate = 1f;
    public float stunDuration = 0.5f;
    public int damage = 1;

    public Transform head;

    private float fireCooldown = 0f;
    private Balloon currentTarget;

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        FindTarget();

        if (currentTarget != null)
        {
            RotateTowardsTarget();

            if (fireCooldown <= 0f)
            {
                ShootElectricRay();
                fireCooldown = 1f / fireRate;
            }
        }
    }

    void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        currentTarget = enemies
            .Select(e => e.GetComponent<Balloon>())
            .Where(b => b != null && Vector3.Distance(transform.position, b.transform.position) <= range)
            .OrderBy(b => Vector3.Distance(transform.position, b.transform.position))
            .FirstOrDefault();
    }

    void RotateTowardsTarget()
    {
        Vector3 dir = currentTarget.transform.position - head.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        head.rotation = Quaternion.Lerp(head.rotation, lookRot, Time.deltaTime * 8f);
    }

    void ShootElectricRay()
    {
        List<Balloon> hitEnemies = new();

        // objetivo principal
        ApplyEffects(currentTarget);
        hitEnemies.Add(currentTarget);

        // obtener todos los enemigos
        var allEnemies = GameObject.FindGameObjectsWithTag("Enemy")
            .Select(e => e.GetComponent<Balloon>())
            .Where(b => b != null)
            .ToList();

        // enemigos dentro del radio de la cadena
        var chainTargets = allEnemies
            .Where(e => !hitEnemies.Contains(e))
            .Where(e => Vector3.Distance(currentTarget.transform.position, e.transform.position) <= chainRange)
            .Take(maxChainTargets);

        // aplicar stun y daño a cada extra
        foreach (var enemy in chainTargets)
        {
            ApplyEffects(enemy);
            hitEnemies.Add(enemy);

            Debug.DrawLine(currentTarget.transform.position, enemy.transform.position, Color.cyan, 0.2f);
        }

        // rayo principal
        Debug.DrawLine(head.position, currentTarget.transform.position, Color.yellow, 0.2f);
    }

    void ApplyEffects(Balloon enemy)
    {
        enemy.TakeDamage(damage);
        enemy.ApplyStun(stunDuration);
    }
}
