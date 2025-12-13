using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TurretShopItem : MonoBehaviour
{
    public GameObject turretPrefab;
    public VRInventory inventory;

    private XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnSelect);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnSelect);
    }

    private void OnSelect(SelectEnterEventArgs args)
    {
        Debug.Log("CLICK EN BOTÓN: " + gameObject.name);

        if (inventory == null)
        {
            Debug.LogError($"[{name}] Inventory NO asignado en el Inspector.");
            return;
        }

        if (turretPrefab == null)
        {
            Debug.LogError($"[{name}] Turret Prefab NO asignado en el Inspector.");
            return;
        }

        inventory.AddItem(turretPrefab);
    }
}
