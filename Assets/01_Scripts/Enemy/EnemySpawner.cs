using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs de enemigos")]
    public GameObject balloonPrefab;     // Globo
    public GameObject blimpPrefab;       // Dirigible

    [Header("Spawn Settings")]
    public float spawnInterval = 1.5f;
    [Range(0f, 1f)] public float balloonProbability = 0.7f; // 70% globo / 30% dirigible

    private void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), spawnInterval, spawnInterval);
    }

    public void SpawnEnemy()
    {
        GameObject prefabToSpawn;

        // Probabilidad
        if (Random.value < balloonProbability)
            prefabToSpawn = balloonPrefab;
        else
            prefabToSpawn = blimpPrefab;

        Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
    }
}
