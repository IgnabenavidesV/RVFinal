using UnityEngine;

public class DirigibleSpawner : MonoBehaviour
{
    [Header("Dirigibles a spawnear")]
    public GameObject dirigiblePrefab;

    [Header("Puntos de Spawn")]
    public Transform[] spawnPoints;   // Asigna aquí 4 puntos o los que quieras

    [Header("Spawn Settings")]
    public float spawnInterval = 5f;
    public int maxDirigibles = 5;

    private float timer;
    private int spawnedCount;

    void Update()
    {
        if (spawnedCount >= maxDirigibles) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnDirigible();
        }
    }

    void SpawnDirigible()
    {
        if (dirigiblePrefab == null || spawnPoints.Length == 0) return;

        // 🔥 Spawn aleatorio
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(dirigiblePrefab, point.position, point.rotation);

        spawnedCount++;
    }
}
