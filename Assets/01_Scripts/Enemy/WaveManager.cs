using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Spawner")]
    public EnemySpawner spawner;
    private bool bossSpawned = false;

    [Header("Wave Settings")]
    public int currentWave = 1;
    public int baseEnemiesPerWave = 5;
    public float spawnInterval = 0.5f;

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

        GameObject wpParent = GameObject.Find("Waypoints");
        if (wpParent != null)
        {
            waypoints = new Transform[wpParent.transform.childCount];
            for (int i = 0; i < wpParent.transform.childCount; i++)
                waypoints[i] = wpParent.transform.GetChild(i);
        }

        AudioManager.Instance.PlayBackgroundMusic();
        StartCoroutine(RunWave());
    }

    private IEnumerator RunWave()
    {
        bossSpawned = false;
        int enemiesToSpawn = baseEnemiesPerWave + Mathf.RoundToInt(currentWave * 2f);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }

        yield return new WaitUntil(() => enemiesAlive <= 0);

        AudioManager.Instance.PlayBackgroundMusic();

        yield return new WaitForSeconds(1f);
        currentWave++;
        StartCoroutine(RunWave());
    }

    private void SpawnEnemy()
    {
        GameObject enemyGO = spawner.SpawnEnemyByWave(currentWave, bossSpawned);
        if (enemyGO == null) return;

        enemiesAlive++;

        EnemyEntry entry = GetEnemyEntry(enemyGO);
        if (entry != null && entry.isBoss && !bossSpawned)
        {
            bossSpawned = true;
            AudioManager.Instance.PlayBossMusic();
        }

        Balloon balloon = enemyGO.GetComponent<Balloon>();
        if (balloon != null)
        {
            float waveFactor = currentWave - 1;
            balloon.speed *= 1f + (speedPerWave * waveFactor);
            balloon.health = Mathf.RoundToInt(balloon.health * (1f + (lifePerWave * waveFactor)));

            if (waypoints != null) balloon.path = waypoints;
            balloon.waveManager = this;
            return;
        }

        Enemy enemy = enemyGO.GetComponent<Enemy>();
        if (enemy != null)
        {
            if (waypoints != null) enemy.SetPath(waypoints);
            enemy.waveManager = this;
            return;
        }

        Debug.LogWarning($"Prefab {enemyGO.name} no tiene Balloon ni Enemy.");
    }

    private EnemyEntry GetEnemyEntry(GameObject instance)
    {
        foreach (EnemyEntry e in spawner.enemies)
        {
            if (instance.name.StartsWith(e.prefab.name))
                return e;
        }
        return null;
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;
        if (enemiesAlive < 0) enemiesAlive = 0;
    }
}
