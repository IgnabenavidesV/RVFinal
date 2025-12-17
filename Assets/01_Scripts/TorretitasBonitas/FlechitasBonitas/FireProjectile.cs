using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;
    public float explosionRadius = 3f;

    public float burnDuration = 2f;
    public float burnDPS = 2f;

    public float lifeTime = 3f;

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

    void OnTriggerEnter(Collider col)
    {
        if (!col.CompareTag("Enemy")) return;

        Explode();
        Destroy(gameObject);
    }

    void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            // Enemy normal
            Enemy e = hit.GetComponentInParent<Enemy>();
            if (e != null)
            {
                e.TakeDamage(damage);
                e.ApplyBurn(burnDuration, burnDPS);
                continue;
            }

            // Boss
            Balloon b = hit.GetComponentInParent<Balloon>();
            if (b != null)
            {
                b.TakeDamage(damage);
                b.ApplyBurn(burnDuration, burnDPS);
            }
        }
    }
}
