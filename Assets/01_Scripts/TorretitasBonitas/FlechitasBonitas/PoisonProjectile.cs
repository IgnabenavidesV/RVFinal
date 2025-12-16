using UnityEngine;

public class PoisonProjectile : MonoBehaviour
{
    [Header("Projectile Stats")]
    public float speed = 12f;

    [Header("Damage (Inspector)")]
    public int impactDamage = 2;
    public float poisonDuration = 6f;
    public float poisonDPS = 1.5f;

    [Header("Lifetime")]
    public float lifeTime = 3f;

    [Header("AOE (0 = sin explosion)")]
    public float explosionRadius = 0f;

    private Transform target;

    public void SetTarget(Transform t) => target = t;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        transform.forward = dir;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!col.CompareTag("Enemy")) return;

        // ? aplica por radio si corresponde
        if (explosionRadius > 0f)
            Explode();
        else
            ApplyToOne(col);

        Destroy(gameObject);
    }

    void ApplyToOne(Collider col)
    {
        Enemy e = col.GetComponentInParent<Enemy>();
        if (e != null)
        {
            e.TakeDamage(impactDamage);
            e.ApplyPoison(poisonDuration, poisonDPS);
            return;
        }

        Balloon b = col.GetComponentInParent<Balloon>();
        if (b != null)
        {
            b.TakeDamage(impactDamage);
            b.ApplyPoison(poisonDuration, poisonDPS);
        }
    }

    void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            Enemy e = hit.GetComponentInParent<Enemy>();
            if (e != null)
            {
                e.TakeDamage(impactDamage);
                e.ApplyPoison(poisonDuration, poisonDPS);
                continue;
            }

            Balloon b = hit.GetComponentInParent<Balloon>();
            if (b != null)
            {
                b.TakeDamage(impactDamage);
                b.ApplyPoison(poisonDuration, poisonDPS);
            }
        }
    }
}
