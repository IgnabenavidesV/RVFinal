using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;     // velocidad
    [HideInInspector] public int damage; // se asigna desde el turret, no se ve en el Inspector
    public float lifeTime = 5f;   // tiempo antes de destruirse sola

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // Avanza siempre hacia adelante
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}
