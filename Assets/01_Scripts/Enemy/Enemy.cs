using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 1;
    public int damageToPlayer = 1;
    public float moveSpeed = 2f;

    private int currentHealth;
    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        gameObject.tag = "Enemy";
    }

    private void Update()
    {
        if (isDead) return;

        // Movimiento simple hacia adelante
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // 🔹 Ya NO spawnea nada, solo desaparece
        Destroy(gameObject);
    }

    public void ReachGoal()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"Enemy llegó a la meta y hace {damageToPlayer} de daño");
        Destroy(gameObject);
    }
}
