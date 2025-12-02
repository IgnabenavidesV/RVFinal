using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Spawner")]
    public EnemySpawner spawner;

    [Header("Wave Settings")]
    public int currentWave = 1;
    public int enemiesPerWave = 5;
    public float spawnInterval = 1.5f;
    public float strategyTime = 5f;

    private int enemiesSpawned = 0;
    private int enemiesAlive = 0;

    [Header("Dificultad")]
    public float enemySpeedMultiplier = 0.10f;  // 10% por oleada
    public float enemyLifeMultiplier = 0.20f;   // 20% por oleada
    public float spawnRateMultiplier = 0.05f;   // reduce 0.05 segundos por oleada
    public float strategyReductionPerWave = 0.2f;

    void Start()
    {
        if (spawner == null)
        {
            Debug.LogError("WaveManager: No se asignó el EnemySpawner.");
            return;
        }

        StartCoroutine(RunWave());
    }

    IEnumerator RunWave()
    {
        enemiesSpawned = 0;
        enemiesAlive = 0;

        // SPAWN DE ENEMIGOS
        while (enemiesSpawned < enemiesPerWave)
        {
            SpawnEnemy();
            enemiesSpawned++;
            yield return new WaitForSeconds(spawnInterval);
        }

        // ESPERAR A QUE MUERAN TODOS
        yield return new WaitUntil(() => enemiesAlive <= 0);

        // TIEMPO EXTRA
        yield return new WaitForSeconds(strategyTime);

        // SIGUIENTE WAVE
        currentWave++;
        enemiesPerWave += 2;

        // AUMENTAR DIFICULTAD
        spawnInterval = Mathf.Max(0.2f, spawnInterval - spawnRateMultiplier);
        strategyTime = Mathf.Max(0f, strategyTime - strategyReductionPerWave);

        StartCoroutine(RunWave());
    }

    public void SpawnEnemy()
    {
        GameObject g = Instantiate(spawner.balloonPrefab, spawner.transform.position, Quaternion.identity);
        Balloon balloon = g.GetComponent<Balloon>();

        if (balloon == null)
        {
            Debug.LogError("El prefab del enemigo no tiene el script Balloon.");
            return;
        }

        // Escalar dificultad por wave
        float waveFactor = currentWave - 1;

        balloon.speed += balloon.speed * (enemySpeedMultiplier * waveFactor);
        balloon.life += Mathf.RoundToInt(balloon.life * (enemyLifeMultiplier * waveFactor));

        balloon.waveManager = this;
        enemiesAlive++;
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;
        if (enemiesAlive < 0)
            enemiesAlive = 0;
    }
}
