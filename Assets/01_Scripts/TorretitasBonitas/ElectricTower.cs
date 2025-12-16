using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElectricTower : MonoBehaviour
{
    public float range = 8f;
    public float chainRange = 4f;
    public int maxChainTargets = 3;
    public float fireRate = 1f;
    public float stunDuration = 0.5f;
    public int damage = 1;

    public Transform head;
    public AudioClip shootAudioClip;
    private AudioSource audioSource;

    private float fireCooldown = 0f;
    private MonoBehaviour currentTarget; // Puede ser Balloon o Enemy

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

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
            .Select(e => e.GetComponent<Balloon>() as MonoBehaviour ?? e.GetComponent<Enemy>())
            .Where(e => e != null && Vector3.Distance(transform.position, e.transform.position) <= range)
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
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
        List<MonoBehaviour> hitEnemies = new();

        ApplyEffects(currentTarget);
        hitEnemies.Add(currentTarget);

        if (shootAudioClip != null)
            audioSource.PlayOneShot(shootAudioClip);

        var allEnemies = GameObject.FindGameObjectsWithTag("Enemy")
            .Select(e => e.GetComponent<Balloon>() as MonoBehaviour ?? e.GetComponent<Enemy>())
            .Where(e => e != null)
            .ToList();

        var chainTargets = allEnemies
            .Where(e => !hitEnemies.Contains(e))
            .Where(e => Vector3.Distance(currentTarget.transform.position, e.transform.position) <= chainRange)
            .Take(maxChainTargets);

        foreach (var enemy in chainTargets)
        {
            ApplyEffects(enemy);
            hitEnemies.Add(enemy);

            Debug.DrawLine(currentTarget.transform.position, enemy.transform.position, Color.cyan, 0.2f);
        }

        Debug.DrawLine(head.position, currentTarget.transform.position, Color.yellow, 0.2f);
    }

    void ApplyEffects(MonoBehaviour enemy)
    {
        // --- Balloon ---
        Balloon b = enemy.GetComponent<Balloon>();
        if (b != null)
        {
            b.TakeDamage(damage);
            b.ApplyStun(stunDuration);
            b.ApplySlow(0.5f, 2f);    // ejemplo de slow
            b.ApplyBurn(3f, 1f);      // ejemplo de burn
            b.ApplyPoison(4f, 1f);    // ejemplo de poison
            return;
        }

        // --- Enemy ---
        Enemy e = enemy.GetComponent<Enemy>();
        if (e != null)
        {
            e.TakeDamage(damage);

            // Para stun y slow en Enemy, agrega métodos similares a Balloon
            e.ApplyStun(stunDuration);
            e.ApplySlow(0.5f, 2f);
            e.ApplyBurn(3f, 1f);
            e.ApplyPoison(4f, 1f);
        }
    }
}
