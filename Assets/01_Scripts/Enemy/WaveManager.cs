using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public EnemySpawner spawner;

    public int currentWave = 1;
    public int enemiesPerWave = 5;
    public float spawnInterval = 1.5f;
    public float strategyTime = 5f; // tiempo extra al finalizar wave

    private int enemiesSpawned = 0;
    private int enemiesAlive = 0;

    void Start()
    {
        StartCoroutine(RunWave());
    }

    IEnumerator RunWave()
    {
        enemiesSpawned = 0;
        enemiesAlive = 0;

        // Spawn enemigos
        while (enemiesSpawned < enemiesPerWave)
        {
            SpawnEnemy();
            enemiesSpawned++;
            yield return new WaitForSeconds(spawnInterval);
        }

        // Esperar hasta que TODOS los globos mueran
        yield return new WaitUntil(() => enemiesAlive == 0);

        // Dar tiempo para estrategia
        yield return new WaitForSeconds(strategyTime);

        // Configurar siguiente wave
        currentWave++;
        enemiesPerWave += 2;  // Aumenta dificultad

        StartCoroutine(RunWave());
    }

    public void SpawnEnemy()
    {
        GameObject g = Instantiate(spawner.balloonPrefab, spawner.transform.position, Quaternion.identity);

        Balloon balloon = g.GetComponent<Balloon>();
        balloon.waveManager = this;

        enemiesAlive++;
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;
    }
}
