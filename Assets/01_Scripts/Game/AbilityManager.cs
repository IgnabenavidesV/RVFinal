using UnityEngine;

public enum AbilityType
{
    None,
    Fireball
}

public class AbilityManager : MonoBehaviour
{
    [Header("Poderes")]
    public AbilityType selectedAbility = AbilityType.None;
    public GameObject fireballPrefab;  // Tu prefab de bola de fuego

    [Header("Grid")]
    public float cellSize = 1f; // Tamaño de tu cuadrícula

    public void SelectFireball()
    {
        selectedAbility = AbilityType.Fireball;
    }

    public void CastSelectedAbility(Vector3 worldPos)
    {
        if (selectedAbility == AbilityType.None) return;

        // Ajustamos la posición al tamaño de la cuadrícula
        worldPos.x = Mathf.Round(worldPos.x / cellSize) * cellSize;
        worldPos.z = Mathf.Round(worldPos.z / cellSize) * cellSize;

        // Instanciamos la bola de fuego y la lanzamos
        if (selectedAbility == AbilityType.Fireball)
        {
            Vector3 spawnPos = worldPos + Vector3.up * 15f;  // Asegúrate de ajustar la altura inicial
            Instantiate(fireballPrefab, spawnPos, Quaternion.identity);
        }
    }
}
