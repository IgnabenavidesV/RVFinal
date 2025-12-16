using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Spawner")]
    public EnemySpawner spawner;

    [Header("Cantidad de enemigos por wave (AQUÍ controlas)")]
    public int enemiesPerWave = 6;

    [Header("Timing")]
    public float spawnInterval = 0.5f;

    [Header("Escalado por wave")]
    public float speedPerWave = 0.08f;
    public float healthPerWave = 0.15f;

    public int currentWave = 1;

    private int enemiesAlive = 0;
    private Transform[] waypoints;
    private bool bossSpawned = false;

    void Start()
    {
        // Waypoints
        GameObject wpParent = GameObject.Find("Waypoints");
        if (wpParent != null)
        {
            waypoints = new Transform[wpParent.transform.childCount];
            for (int i = 0; i < wpParent.transform.childCount; i++)
                waypoints[i] = wpParent.transform.GetChild(i);
        }

        StartCoroutine(RunWave());
    }

    private IEnumerator RunWave()
    {
        bossSpawned = false;

        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }

        yield return new WaitUntil(() => enemiesAlive <= 0);

        currentWave++;
        yield return new WaitForSeconds(1f);
        StartCoroutine(RunWave());
    }

    private void SpawnEnemy()
    {
        var enemyGO = spawner.SpawnEnemyByWave(currentWave, bossSpawned);
        if (enemyGO == null) return;

        enemiesAlive++;

        // Boss
        var boss = enemyGO.GetComponent<Balloon>();
        if (boss != null)
        {
            bossSpawned = true;

            float waveFactor = currentWave - 1;
            boss.speed *= 1f + (speedPerWave * waveFactor);
            boss.health = Mathf.RoundToInt(boss.health * (1f + (healthPerWave * waveFactor)));

            if (waypoints != null) boss.path = waypoints;
            boss.waveManager = this;
            return;
        }

        // Enemy normal
        var enemy = enemyGO.GetComponent<Enemy>();
        if (enemy != null)
        {
            float waveFactor = currentWave - 1;
            enemy.moveSpeed *= 1f + (speedPerWave * waveFactor);
            enemy.health = Mathf.RoundToInt(enemy.health * (1f + (healthPerWave * waveFactor)));

            if (waypoints != null) enemy.SetPath(waypoints);
            enemy.waveManager = this;
            return;
        }

        Debug.LogWarning($"Prefab {enemyGO.name} no tiene Enemy ni Balloon.");
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;
        if (enemiesAlive < 0) enemiesAlive = 0;
    }
}
