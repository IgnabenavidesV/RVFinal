using UnityEngine;

public class PoisonProjectile : MonoBehaviour
{
    public float speed = 12f;
    public int impactDamage = 2;
    public float poisonDuration = 6f;
    public float poisonDPS = 1.5f;
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
            b.TakeDamage(impactDamage);
            b.ApplyPoison(poisonDuration, poisonDPS);
        }

        Destroy(gameObject);
    }
}
