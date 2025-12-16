using UnityEngine;

public class PoisonProjectile : MonoBehaviour
{
    [Header("Projectile Stats")]
    public float speed = 12f;
    public int impactDamage = 2;
    public float poisonDuration = 6f;
    public float poisonDPS = 1.5f;
    public float lifeTime = 3f;

    public float explosionRadius = 0f; // Si >0, aplica a enemigos cercanos

    private Transform target;

    public void SetTarget(Transform t)
    {
        target = t;
    }

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

        // Enemy normal
        Enemy e = col.GetComponentInParent<Enemy>();
        if (e != null)
        {
            e.TakeDamage(impactDamage);
            e.ApplyPoison(poisonDuration, poisonDPS);
            Destroy(gameObject);
            return;
        }

        // Boss o Balloon
        Balloon b = col.GetComponentInParent<Balloon>();
        if (b != null)
        {
            b.TakeDamage(impactDamage);
            b.ApplyPoison(poisonDuration, poisonDPS);
            Destroy(gameObject);
        }
    }


    void Explode()
    {
        Collider[] hits;

        if (explosionRadius > 0f)
        {
            hits = Physics.OverlapSphere(transform.position, explosionRadius);
        }
        else
        {
            hits = new Collider[] { Physics.OverlapSphere(transform.position, 0.1f)[0] };
        }

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            // Enemigo normal
            Enemy e = hit.GetComponentInParent<Enemy>();
            if (e != null)
            {
                e.TakeDamage(impactDamage);
                e.ApplyPoison(poisonDuration, poisonDPS);
                continue;
            }

            // Boss o balloon
            Balloon b = hit.GetComponentInParent<Balloon>();
            if (b != null)
            {
                b.TakeDamage(impactDamage);
                b.ApplyPoison(poisonDuration, poisonDPS);
            }
        }
    }
}
