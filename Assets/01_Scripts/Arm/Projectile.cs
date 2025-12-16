using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 30f;
    public float damage = 10f;
    public float lifeTime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // ?? SOLO daña al Boss
        //if (other.CompareTag("Boss"))
        //{
        //    BossHealth boss = other.GetComponent<BossHealth>();
        //    if (boss != null)
        //        boss.TakeDamage(damage);
        //}

        // El proyectil desaparece SIEMPRE al impactar
        Destroy(gameObject);
    }
}
