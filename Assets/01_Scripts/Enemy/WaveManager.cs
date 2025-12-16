using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Spawner")]
    public EnemySpawner spawner;

    [Header("Wave Settings")]
    public int currentWave = 1;
    public int baseEnemiesPerWave = 5;
    public float spawnInterval = 0.5f; // tiempo entre spawn de cada enemigo

    [Header("Difficulty Scaling")]
    public float speedPerWave = 0.08f;
    public float lifePerWave = 0.15f;

    private int enemiesAlive = 0;
    private Transform[] waypoints;

    void Start()
    {
        if (spawner == null)
        {
            Debug.LogError("WaveManager: EnemySpawner no asignado.");
            return;
        }

        // Obtener waypoints
        GameObject wpParent = GameObject.Find("Waypoints");
        if (wpParent != null)
        {
            waypoints = new Transform[wpParent.transform.childCount];
            for (int i = 0; i < wpParent.transform.childCount; i++)
            {
                waypoints[i] = wpParent.transform.GetChild(i);
            }
        }

        StartCoroutine(RunWave());
    }

    private IEnumerator RunWave()
    {
        int enemiesToSpawn = baseEnemiesPerWave + Mathf.RoundToInt(currentWave * 2f);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }

        // Esperar hasta que todos los enemigos estén muertos o hayan llegado a meta
        yield return new WaitUntil(() => enemiesAlive <= 0);

        // 1 segundo de pausa y siguiente wave
        yield return new WaitForSeconds(1f);

        currentWave++;
        StartCoroutine(RunWave());
    }

    private void SpawnEnemy()
    {
        GameObject enemyGO = spawner.SpawnEnemyByWave(currentWave);
        if (enemyGO == null) return;

        enemiesAlive++;

        // --- Balloon ---
        Balloon balloon = enemyGO.GetComponent<Balloon>();
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
        Enemy enemy = enemyGO.GetComponent<Enemy>();
        if (enemy != null)
        {
            if (waypoints != null) enemy.SetPath(waypoints);
            enemy.waveManager = this;
            return;
        }

        Debug.LogWarning($"Prefab {enemyGO.name} no tiene Balloon ni Enemy.");
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;
        if (enemiesAlive < 0) enemiesAlive = 0;
    }
}
