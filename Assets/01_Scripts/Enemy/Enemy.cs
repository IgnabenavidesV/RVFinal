using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 1;
    public int damageToPlayer = 1;
    public float moveSpeed = 2f;

    [Header("Waypoint Movement")]
    public Transform[] path; // Ruta a seguir
    private int currentPoint = 0;

    [Header("Floating Animation")]
    public float floatAmplitudeY = 0.5f; // altura vertical
    public float floatFrequencyY = 0.8f; // velocidad vertical
    public float floatAmplitudeX = 0.2f; // desplazamiento horizontal
    public float floatFrequencyX = 0.5f; // velocidad horizontal
    public float rotationSpeed = 20f;    // rotación lenta

    private Vector3 startPos;
    private int currentHealth;
    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        gameObject.tag = "Enemy";
        startPos = transform.position;
    }

    private void Update()
    {
        if (isDead) return;

        if (path != null && path.Length > 0)
        {
            MoveAlongPath();
        }

        AnimateFloatingGod();
    }

    private void MoveAlongPath()
    {
        Transform target = path[currentPoint];

        // Movimiento hacia waypoint
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // Rotación suave hacia el objetivo
        Vector3 dir = (target.position - transform.position).normalized;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 2f);

        // Llegó al waypoint
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentPoint++;
            if (currentPoint >= path.Length)
            {
                ReachGoal();
            }
        }
    }

    private void AnimateFloatingGod()
    {
        float time = Time.time;

        // Flotación vertical
        float yOffset = Mathf.Sin(time * floatFrequencyY) * floatAmplitudeY;

        // Oscilación horizontal suave
        float xOffset = Mathf.Sin(time * floatFrequencyX) * floatAmplitudeX;

        transform.position += new Vector3(xOffset, yOffset, 0f) * Time.deltaTime;

        // Rotación suave tipo globo
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.forward, rotationSpeed / 2f * Time.deltaTime, Space.World);
    }

    public void SetPath(Transform[] waypoints)
    {
        path = waypoints;
        currentPoint = 0;
        startPos = transform.position;
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
