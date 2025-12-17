using UnityEngine;

public class IceProjectile : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 15f;
    public float lifeTime = 3f;

    [Header("Damage")]
    public int damage = 1;

    [Header("Ice Effect")]
    [Range(0f, 1f)] public float slowPercent = 0.5f;
    public float slowDuration = 2f;

    private Transform target;

    public void SetTarget(Transform t) => target = t;

    private void Start() => Destroy(gameObject, lifeTime);

    private void Update()
    {
        if (target == null) { Destroy(gameObject); return; }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        transform.forward = dir;
    }

    private void OnTriggerEnter(Collider col)
    {
        Debug.Log($"[Projectile] HIT collider={col.name}");

        // 1) Enemy normal
        Enemy enemy = col.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            enemy.ApplySlow(slowPercent, slowDuration);
            Destroy(gameObject);
            return;
        }

        // 2) Boss
        Balloon boss = col.GetComponentInParent<Balloon>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            boss.ApplySlow(slowPercent, slowDuration);
            Destroy(gameObject);
            return;
        }
    }
}
