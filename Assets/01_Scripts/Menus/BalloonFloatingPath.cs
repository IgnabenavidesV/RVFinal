using System.Collections.Generic;
using UnityEngine;

public class BalloonFloatingPath : MonoBehaviour
{
    [Header("Balloons (pueden ser PREFABS o instancias en escena)")]
    public Transform[] balloons;

    [Header("Path points (si lo dejas vacío, usa los HIJOS de este objeto)")]
    public Transform[] points;

    [Header("Movement")]
    public float speed = 1.2f;
    public bool loop = true;
    public float arriveDistance = 0.15f;

    [Header("Floating (visual)")]
    public float floatAmplitudeY = 0.5f;
    public float floatFrequencyY = 0.8f;
    public float floatAmplitudeX = 0.2f;
    public float floatFrequencyX = 0.5f;

    [Header("Rotation")]
    public float rotationSpeed = 20f;

    [Header("Spawn / Debug")]
    public bool spawnFromPrefabs = true;
    public bool disableOtherMovementScripts = true;
    public bool parentSpawnedToThis = false;

    // runtime
    List<Transform> runtimeBalloons = new List<Transform>();
    Vector3[] basePos;
    int[] targetIndex;
    float[] phase;

    void Awake()
    {
        // 1) Points: si no asignaste manualmente, toma los hijos del BalloonPath
        if (points == null || points.Length == 0)
        {
            var temp = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var c = transform.GetChild(i);
                // evita agarrar objetos que no son puntos (si tuvieras otros hijos)
                temp.Add(c);
            }
            points = temp.ToArray();
        }

        if (points.Length < 2)
        {
            Debug.LogError("[BalloonFloatingPath] Necesitas al menos 2 puntos en el path.");
            enabled = false;
            return;
        }

        // 2) Spawn/usar balloons
        PrepareBalloons();

        if (runtimeBalloons.Count == 0)
        {
            Debug.LogError("[BalloonFloatingPath] No hay balloons para mover. Asigna 4 prefabs/instancias en 'balloons'.");
            enabled = false;
            return;
        }

        // 3) init arrays
        basePos = new Vector3[runtimeBalloons.Count];
        targetIndex = new int[runtimeBalloons.Count];
        phase = new float[runtimeBalloons.Count];

        // 4) Distribuirlos en puntos diferentes (para que no nazcan apilados)
        for (int i = 0; i < runtimeBalloons.Count; i++)
        {
            int startIdx = i % points.Length;
            targetIndex[i] = (startIdx + 1) % points.Length;

            basePos[i] = points[startIdx].position;
            runtimeBalloons[i].position = basePos[i];

            // fase distinta para que no floten igual
            phase[i] = i * 1.37f;
        }
    }

    void PrepareBalloons()
    {
        runtimeBalloons.Clear();

        if (balloons == null) return;

        for (int i = 0; i < balloons.Length; i++)
        {
            if (balloons[i] == null) continue;

            Transform t = balloons[i];

            // Si es prefab (no está en escena) y spawnFromPrefabs está ON -> instanciarlo
            bool isSceneObject = t.gameObject.scene.IsValid();
            if (spawnFromPrefabs && !isSceneObject)
            {
                var go = Instantiate(t.gameObject);
                if (parentSpawnedToThis) go.transform.SetParent(transform, true);
                t = go.transform;
            }

            if (disableOtherMovementScripts)
            {
                // Esto evita que tu Enemy (Waypoint Movement) pelee con el path
                var enemy = t.GetComponent<MonoBehaviour>(); // placeholder para evitar warning
                // desactiva el script Enemy si existe
                var enemyScript = t.GetComponent<Enemy>();
                if (enemyScript != null) enemyScript.enabled = false;

                // si tienes otros scripts que muevan, aquí puedes agregarlos:
                // var other = t.GetComponent<OtroMover>(); if (other != null) other.enabled = false;

                // si tiene rigidbody, mejor dejarlo kinematic para que no “se caiga” o choque raro
                var rb = t.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
            }

            runtimeBalloons.Add(t);
        }
    }

    void Update()
    {
        float tTime = Time.time;

        for (int i = 0; i < runtimeBalloons.Count; i++)
        {
            Transform b = runtimeBalloons[i];
            if (b == null) continue;

            Vector3 target = points[targetIndex[i]].position;

            // 1) Movimiento REAL por el path (basePos) - esto es lo que cuenta para llegar al punto
            basePos[i] = Vector3.MoveTowards(basePos[i], target, speed * Time.deltaTime);

            // 2) Flotación VISUAL (no afecta la lógica de llegar)
            float fy = Mathf.Sin((tTime + phase[i]) * floatFrequencyY) * floatAmplitudeY;
            float fx = Mathf.Sin((tTime + phase[i]) * floatFrequencyX) * floatAmplitudeX;

            // offset lateral suave perpendicular a la dirección (para que no sea tan lineal)
            Vector3 dir = (target - basePos[i]);
            Vector3 right = (dir.sqrMagnitude > 0.0001f) ? Vector3.Cross(Vector3.up, dir.normalized) : Vector3.right;

            Vector3 visualPos = basePos[i] + Vector3.up * fy + right * fx;
            b.position = visualPos;

            // 3) Rotación
            b.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

            // 4) Llegó al punto? (compara con basePos, NO con visualPos)
            if (Vector3.Distance(basePos[i], target) <= arriveDistance)
            {
                int next = targetIndex[i] + 1;

                if (next >= points.Length)
                {
                    if (loop) next = 0;
                    else
                    {
                        // si no loopea, se queda en el último
                        targetIndex[i] = points.Length - 1;
                        continue;
                    }
                }

                targetIndex[i] = next;
            }
        }
    }
}
