using UnityEngine;
using System.Collections;

public class TurretHeadScanner : MonoBehaviour
{
    [Header("Scan")]
    public float angleStep = 45f;          // 45 grados por paso
    public float rotationSpeed = 120f;     // grados/seg
    public Vector2 waitRange = new Vector2(1.2f, 3.0f);

    float _targetY;
    Coroutine _co;

    void OnEnable()
    {
        _targetY = transform.localEulerAngles.y;
        _co = StartCoroutine(ScanLoop());
    }

    void OnDisable()
    {
        if (_co != null) StopCoroutine(_co);
    }

    IEnumerator ScanLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(waitRange.x, waitRange.y));

            float dir = Random.value > 0.5f ? 1f : -1f;
            _targetY += angleStep * dir;

            Quaternion target = Quaternion.Euler(0f, _targetY, 0f);

            // rota suave hasta llegar
            while (Quaternion.Angle(transform.localRotation, target) > 0.5f)
            {
                transform.localRotation = Quaternion.RotateTowards(
                    transform.localRotation,
                    target,
                    rotationSpeed * Time.deltaTime
                );
                yield return null;
            }

            transform.localRotation = target;
        }
    }
}
