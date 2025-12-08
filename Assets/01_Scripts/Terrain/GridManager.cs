using UnityEngine;

[ExecuteAlways]
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Settings")]
    public int cellsX = 25;        // 25 celdas en X
    public int cellsZ = 25;        // 25 celdas en Z
    public float cellSize = 4f;    // cada celda mide 4 unidades
    public Color gridColor = new Color(1f, 1f, 1f, 0.25f);

    public Vector3 Origin => transform.position;

    private void Awake()
    {
        // para poder usar GridManager.Instance en otros scripts
        if (!Application.isPlaying)
            return;
        Instance = this;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gridColor;
        Vector3 origin = Origin;

        // líneas verticales
        for (int x = 0; x <= cellsX; x++)
        {
            Vector3 from = origin + new Vector3(x * cellSize, 0f, 0f);
            Vector3 to = origin + new Vector3(x * cellSize, 0f, cellsZ * cellSize);
            Gizmos.DrawLine(from, to);
        }

        // líneas horizontales
        for (int z = 0; z <= cellsZ; z++)
        {
            Vector3 from = origin + new Vector3(0f, 0f, z * cellSize);
            Vector3 to = origin + new Vector3(cellsX * cellSize, 0f, z * cellSize);
            Gizmos.DrawLine(from, to);
        }
    }

    /// <summary>Devuelve la posición centrada en la celda más cercana.</summary>
    public Vector3 SnapToGrid(Vector3 worldPos)
    {
        Vector3 local = worldPos - Origin;

        int cellX = Mathf.RoundToInt(local.x / cellSize);
        int cellZ = Mathf.RoundToInt(local.z / cellSize);

        float snappedX = cellX * cellSize;
        float snappedZ = cellZ * cellSize;

        return new Vector3(Origin.x + snappedX, worldPos.y, Origin.z + snappedZ);
    }

    /// <summary>Convierte posición mundial a coordenadas de celda.</summary>
    public Vector2Int WorldToCell(Vector3 worldPos)
    {
        Vector3 local = worldPos - Origin;
        int cellX = Mathf.RoundToInt(local.x / cellSize);
        int cellZ = Mathf.RoundToInt(local.z / cellSize);
        return new Vector2Int(cellX, cellZ);
    }

    /// <summary>Convierte coordenadas de celda a centro de celda en mundo.</summary>
    public Vector3 CellToWorldCenter(Vector2Int cell, float y = 0f)
    {
        float x = Origin.x + cell.x * cellSize;
        float z = Origin.z + cell.y * cellSize;
        return new Vector3(x, y, z);
    }
}
