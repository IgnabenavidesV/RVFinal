using UnityEngine;

public class MinimapManager : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject minimapCanvas;   // arrastra el Canvas aquí

    void Start()
    {
        minimapCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2)) // Cambia el minimapa con la tecla 1
        {
            bool active = !minimapCanvas.activeSelf;
            minimapCanvas.SetActive(active);

            // Mostrar/ocultar el cursor (opcional, para que no se vea en RV)
            Cursor.visible = active;
            Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
