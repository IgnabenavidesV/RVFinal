using UnityEngine;

public class DirigibleSpawner : MonoBehaviour
{
    [Header("Globos que va soltando")]
    public GameObject balloonPrefab;       // globo que va a soltar (Balloon, Balloon2, etc.)
    public Transform spawnPoint;           // punto desde donde salen los globos
    public float spawnInterval = 2f;       // cada cuántos segundos suelta uno
    public int maxBalloonsToSpawn = 10;    // cuántos como máximo

    private float timer;
    private int spawnedCount;

    private void Update()
    {
        if (spawnedCount >= maxBalloonsToSpawn) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnBalloon();
        }
    }

    private void SpawnBalloon()
    {
        if (balloonPrefab == null) return;

        Transform point = spawnPoint != null ? spawnPoint : transform;

        Instantiate(balloonPrefab, point.position, point.rotation);
        spawnedCount++;
    }
}
