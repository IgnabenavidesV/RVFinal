using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Spawner")]
    public EnemySpawner spawner;

    [Header("Wave Settings")]
    public int currentWave = 1;
    public int baseEnemiesPerWave = 5;
    public float baseSpawnInterval = 1.5f;
    public float baseStrategyTime = 5f;

    [Header("Difficulty Scaling")]
    public float enemiesIncreasePerWave = 2f;
    public float spawnIntervalReduction = 0.07f;
    public float strategyReduction = 0.3f;
    public float speedPerWave = 0.08f;
    public float lifePerWave = 0.15f;

    [Header("Limits")]
    public float minSpawnInterval = 0.3f;
    public float minStrategyTime = 1f;

    private int enemiesAlive = 0;

    private Transform[] waypoints;

    void Start()
    {
        if (spawner == null)
        {
            Debug.LogError("WaveManager: EnemySpawner no asignado.");
            return;
        }

        // Obtener waypoints una sola vez
        var wpParent = GameObject.Find("Waypoints");
        if (wpParent != null)
        {
            waypoints = new Transform[wpParent.transform.childCount];
            for (int i = 0; i < wpParent.transform.childCount; i++)
            {
                waypoints[i] = wpParent.transform.GetChild(i);
            }
        }
        else
        {
            Debug.LogError("WaveManager: No se encontró GameObject 'Waypoints'.");
        }

        StartCoroutine(RunWave());
    }

    IEnumerator RunWave()
    {
        enemiesAlive = 0;

        int enemiesToSpawn = baseEnemiesPerWave + Mathf.RoundToInt(currentWave * enemiesIncreasePerWave);

        float spawnInterval = Mathf.Max(minSpawnInterval, baseSpawnInterval - currentWave * spawnIntervalReduction);
        float strategyTime = Mathf.Max(minStrategyTime, baseStrategyTime - currentWave * strategyReduction);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }

        yield return new WaitUntil(() => enemiesAlive <= 0);
        yield return new WaitForSeconds(strategyTime);

        currentWave++;
        StartCoroutine(RunWave());
    }

    void SpawnEnemy()
    {
        GameObject enemyGO = spawner.SpawnEnemyByWave(currentWave);
        if (enemyGO == null) return;

        enemiesAlive++;

        // --- Balloon ---
        Balloon balloon = enemyGO.GetComponent<Balloon>();
        Enemy enemy = enemyGO.GetComponent<Enemy>();

        // Detecta si es jefe
        EnemyEntry entry = spawner.enemies.Find(e => e.prefab == enemyGO);
        if (entry != null && entry.isBoss)
        {
            AudioManager.Instance.PlayBossMusic();
        }
        if (balloon != null)
        {
            float waveFactor = currentWave - 1;
            balloon.speed *= 1f + (speedPerWave * waveFactor);
            balloon.life = Mathf.RoundToInt(balloon.life * (1f + (lifePerWave * waveFactor)));

            if (waypoints != null) balloon.path = waypoints;
            balloon.waveManager = this;
            return;
        }

        // --- Enemy ---
        if (enemy != null)
        {
            if (waypoints != null) enemy.SetPath(waypoints);
            return;
        }

        // Si no es ninguno de los dos
        Debug.LogWarning($"Prefab {enemyGO.name} no tiene Balloon ni Enemy.");
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;
        if (enemiesAlive < 0) enemiesAlive = 0;
    }
}
