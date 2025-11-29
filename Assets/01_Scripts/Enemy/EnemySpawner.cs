using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject balloonPrefab;
    public float spawnInterval = 1.5f;

    public void SpawnEnemy()
    {
        Instantiate(balloonPrefab, transform.position, Quaternion.identity);
    }
}
