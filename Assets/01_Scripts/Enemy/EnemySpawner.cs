using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject balloonPrefab;
    public float spawnInterval = 2f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            Instantiate(balloonPrefab, transform.position, Quaternion.identity);
            timer = 0;
        }
    }
}
