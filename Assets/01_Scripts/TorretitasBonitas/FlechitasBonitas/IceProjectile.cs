using UnityEngine;

public class IceProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float slowPercent = 0.5f;   // 50% menos velocidad
    public float slowDuration = 2f;    // durante 2s
    public int damage = 1;
    public float lifeTime = 3f;

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

        Balloon b = col.GetComponent<Balloon>();

        if (b != null)
        {
            b.ApplySlow(slowPercent, slowDuration);
            b.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
