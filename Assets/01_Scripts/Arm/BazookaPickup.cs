using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class BazookaPickup : MonoBehaviour
{
    public GameObject bazookaPrefab;

    private XRSimpleInteractable interactable;
    private bool picked = false;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        // ? IMPORTANTÍSIMO: si el pedestal vuelve a activarse, debe poder pickearse otra vez
        picked = false;

        interactable.selectEntered.AddListener(OnPick);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnPick);
    }

    private void OnPick(SelectEnterEventArgs args)
    {
        if (picked) return;
        picked = true;

        if (WeaponManager.Instance == null) { picked = false; return; }

        if (WeaponManager.Instance.inventory != null &&
            WeaponManager.Instance.inventory.Count > 0)
        {
            Debug.Log("Vacía el inventario para usar la bazooka");
            picked = false; // permitir intentar otra vez
            return;
        }

        WeaponManager.Instance.EquipWeapon(bazookaPrefab, gameObject);
        gameObject.SetActive(false);
    }
}
