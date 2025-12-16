using UnityEngine;
using UnityEngine.EventSystems;

public class MinimapClickHandler : MonoBehaviour, IPointerClickHandler
{
    [Header("Referencias")]
    public Camera minimapCamera;          // Asigna la cámara del minimapa aquí
    public LayerMask groundMask;          // Asigna la capa del terreno aquí
    public AbilityManager abilityManager; // Asigna el script de poderes aquí

    public void OnPointerClick(PointerEventData eventData)
    {
        // Ray desde la cámara del minimapa hacia el punto donde hiciste clic
        Ray ray = minimapCamera.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundMask))  // Asegúrate de que el terreno tenga la capa correspondiente
        {
            abilityManager.CastSelectedAbility(hit.point); // Llama a la función para lanzar el poder (bola de fuego)
        }
    }
}
