using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 30f;
    public float damage = 10f;
    public float lifeTime = 5f;
    public enum FireAxis { ForwardZ, RightX, UpY }
    public FireAxis axis = FireAxis.RightX;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        Vector3 dir = axis switch
        {
            FireAxis.RightX => transform.right,
            FireAxis.UpY => transform.up,
            _ => transform.forward
        };

        transform.position += dir * speed * Time.deltaTime;


    }

    private void OnTriggerEnter(Collider other)
    {
        // SOLO daña al Boss
        if (other.CompareTag("Boss"))
        {
            // Obtener el componente Balloon en vez de BossHealth
            Balloon boss = other.GetComponent<Balloon>();
            if (boss != null)
                boss.TakeDamage(Mathf.RoundToInt(damage));
        }

        // El proyectil desaparece SIEMPRE al impactar
        Destroy(gameObject);
    }

}
