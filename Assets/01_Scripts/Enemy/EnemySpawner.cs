
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyEntry
{
    public GameObject prefab;
    public int minWave = 1;
    [Range(0f, 1f)]
    public float probability = 1f;
    public bool isBoss;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Lista de enemigos")]
    public List<EnemyEntry> enemies = new List<EnemyEntry>();

    public GameObject SpawnEnemyByWave(int currentWave)
    {
        // ======================
        // 1️⃣ BOSS
        // ======================
        foreach (EnemyEntry e in enemies)
        {
            if (e.isBoss && e.prefab != null &&
                currentWave >= e.minWave &&
                currentWave % e.minWave == 0)
            {
                return Instantiate(e.prefab, transform.position, Quaternion.identity);
            }
        }

        // ======================
        // 2️⃣ ENEMIGOS NORMALES
        // ======================
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

        // 🔴 PROTECCIÓN CLAVE
        if (available.Count == 0 || totalWeight <= 0f)
        {
            Debug.LogError(
                $"EnemySpawner: No hay enemigos válidos para la wave {currentWave}. " +
                $"Revisa minWave y probability."
            );
            return null;
        }

        // ======================
        // 3️⃣ SELECCIÓN PONDERADA
        // ======================
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

        // Fallback seguro
        return Instantiate(available[0].prefab, transform.position, Quaternion.identity);
    }
}
