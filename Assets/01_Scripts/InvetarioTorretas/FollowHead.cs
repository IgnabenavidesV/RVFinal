using UnityEngine;

public class FollowHead : MonoBehaviour
{
    [Header("Referencia a la cámara del XR (Main Camera)")]
    public Transform head;

    [Header("Offset respecto a la cabeza (local)")]
    public Vector3 offset = new Vector3(0f, -0.35f, 0.8f); // abajo y delante

    [Header("Suavizado")]
    public float positionLerp = 10f;
    public float rotationLerp = 10f;    

    [Header("Opciones")]
    public bool followRotationYOnly = true; // que rote solo con yaw

    void LateUpdate()
    {
        if (head == null) return;

        // Rotación: solo eje Y para evitar que el inventario se incline al mirar arriba/abajo
        Quaternion rot = head.rotation;
        if (followRotationYOnly)
        {
            Vector3 e = rot.eulerAngles;
            rot = Quaternion.Euler(0f, e.y, 0f);
        }

        // Posición deseada frente a la cabeza
        Vector3 targetPos = head.position + rot * offset;

        // Suavizado
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * positionLerp);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotationLerp);
    }
}
    