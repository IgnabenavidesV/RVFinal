using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class BazookaPickup : MonoBehaviour
{
    public GameObject bazookaPrefab;

    private XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnPick);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnPick);
    }
    private bool picked = false;

    private void OnPick(SelectEnterEventArgs args)
    {
        if (picked) return;
        picked = true;

        Debug.Log("? PICKUP SELECTED");

        if (WeaponManager.Instance == null) return;

        if (WeaponManager.Instance.inventory != null &&
            WeaponManager.Instance.inventory.Count > 0)
        {
            Debug.Log("Vacía el inventario para usar la bazooka");
            picked = false; // para permitir intentarlo luego
            return;
        }

        WeaponManager.Instance.EquipWeapon(bazookaPrefab, gameObject);
        gameObject.SetActive(false);
    }


}
