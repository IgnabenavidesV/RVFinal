using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public float speed = 20f;
    public float damage = 20f;
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
        // Si el enemigo fue destruido, destruimos también la flecha
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // IMPORTANTE: try-catch evita error si el enemy se destruye ENTRE FRAMES
        try
        {
            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime;
            transform.forward = dir;
        }
        catch
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!col.CompareTag("Enemy")) return;

        // TODO: col.GetComponent<Enemy>().TakeDamage(damage);

        Destroy(gameObject);
    }
}
