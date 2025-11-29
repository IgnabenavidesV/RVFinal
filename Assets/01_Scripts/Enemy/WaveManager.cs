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

        // ESPERAR HASTA QUE TODOS MUERAN
        yield return new WaitUntil(() => enemiesAlive <= 0);

        // TIEMPO EXTRA DE ESTRATEGIA
        yield return new WaitForSeconds(strategyTime);

        // SIGUIENTE WAVE
        currentWave++;
        enemiesPerWave += 2;

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

        balloon.waveManager = this;
        enemiesAlive++;
    }

    // LLAMADO DESDE EL ENEMIGO CUANDO MUERE
    public void OnEnemyKilled()
    {
        enemiesAlive--;
        if (enemiesAlive < 0)
            enemiesAlive = 0;
    }
}
