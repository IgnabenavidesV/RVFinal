using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyEntry
{
    [Tooltip("Prefab del enemigo a spawnear.")]
    public GameObject prefab;

    [Tooltip("Wave mínima en la que puede aparecer.")]
    public int minWave = 1;

    [Tooltip("Probabilidad de que este enemigo aparezca (0 a 1).")]
    [Range(0f, 1f)]
    public float probability = 1f;

    [Tooltip("Indica si este enemigo es un jefe.")]
    public bool isBoss = false;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Lista de enemigos disponibles")]
    public List<EnemyEntry> enemies = new List<EnemyEntry>();

    /// <summary>
    /// Spawnea un enemigo adecuado según la wave actual.
    /// </summary>
    /// <param name="currentWave">Número de wave actual</param>
    /// <param name="bossAlreadySpawned">Indica si ya hay un boss en la wave</param>
    /// <returns>GameObject instanciado o null si falla</returns>
    public GameObject SpawnEnemyByWave(int currentWave, bool bossAlreadySpawned = false)
    {
        // 1️⃣ Revisar si toca spawnear un boss
        foreach (EnemyEntry e in enemies)
        {
            if (e.isBoss && e.prefab != null &&
                currentWave >= e.minWave &&
                currentWave % e.minWave == 0 &&
                !bossAlreadySpawned)
            {
                return Instantiate(e.prefab, transform.position, Quaternion.identity);
            }
        }

        // 2️⃣ Filtrar enemigos normales
        List<EnemyEntry> available = new List<EnemyEntry>();
        float totalWeight = 0f;

        foreach (EnemyEntry e in enemies)
        {
            if (!e.isBoss && e.prefab != null && currentWave >= e.minWave)
            {
                available.Add(e);
                totalWeight += Mathf.Max(0.0001f, e.probability);
            }
        }

        if (available.Count == 0)
        {
            Debug.LogError($"EnemySpawner: No hay enemigos válidos para la wave {currentWave}");
            return null;
        }

        // 3️⃣ Selección aleatoria ponderada
        float rand = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        foreach (EnemyEntry e in available)
        {
            cumulative += Mathf.Max(0.0001f, e.probability);
            if (rand <= cumulative)
            {
                return Instantiate(e.prefab, transform.position, Quaternion.identity);
            }
        }

        // 4️⃣ Fallback seguro
        return Instantiate(available[0].prefab, transform.position, Quaternion.identity);
    }
}
