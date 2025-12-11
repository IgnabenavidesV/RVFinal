using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 10f;  // Velocidad de la bola de fuego
    public float lifeTime = 5f; // Tiempo de vida antes de destruirse
    public float damage = 50f; // Daño que causa al impactar

    private float timer;

    void Start()
    {
        // La bola de fuego tiene un tiempo de vida
        timer = lifeTime;
    }

    void Update()
    {
        // Movimiento hacia abajo o lo que desees (puedes cambiar la dirección)
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // Disminuir el tiempo de vida y destruir la bola después de cierto tiempo
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Detecta colisiones con otros objetos
    void OnTriggerEnter(Collider other)
    {
        // Si la bola de fuego toca un enemigo o un objeto, causar daño o efectos
        if (other.CompareTag("Enemy"))
        {
            // Aquí puedes aplicar daño al enemigo
            Debug.Log("¡Daño a enemigo!");

            // Destruir la bola de fuego al impactar
            Destroy(gameObject);
        }
    }
}
