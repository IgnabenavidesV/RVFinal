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

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
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

        Explode();

        Destroy(gameObject);
    }

    private void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Balloon enemy = hit.GetComponent<Balloon>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    enemy.ApplyBurn(burnDuration, burnDPS);
                }
            }
        }

        // Debug visual del área
        Debug.DrawRay(transform.position, Vector3.up * 2f, Color.red, 0.5f);
    }
}
