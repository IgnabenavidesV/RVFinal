using UnityEngine;

public class EndPoint : MonoBehaviour
{
    public int damagePerEnemy = 1; // daño que recibe el jugador al llegar un enemigo

    void OnTriggerEnter(Collider other)
    {
        // Verifica que el objeto que llega es un enemigo
        if (other.CompareTag("Enemy"))
        {
            // Daño al jugador
            GameManager.Instance.TakeDamage(damagePerEnemy);

            // Destruye el enemigo que llegó al punto
            Destroy(other.gameObject);
        }
    }
}
