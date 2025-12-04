using UnityEngine;

public class BlimpBullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tower"))
        {
            // Aquí debes tener un script de vida en las torretas
            // Ejemplo:
            TowerHealth tower = other.GetComponent<TowerHealth>();
            if (tower != null) tower.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
