using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class BazookaPickup : MonoBehaviour
{
    public GameObject bazookaPrefab;

    private XRSimpleInteractable interactable;
    private bool picked;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        picked = false; // ? IMPORTANTE: permitir volver a pickear
        interactable.selectEntered.AddListener(OnPick);
        interactable.enabled = true; // por si algo lo deshabilitó
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnPick);
    }

    private void OnPick(SelectEnterEventArgs args)
    {
        if (picked) return;  // ? evita doble pick
        picked = true;

        if (WeaponManager.Instance == null) return;

        WeaponManager.Instance.EquipWeapon(bazookaPrefab, gameObject);

        gameObject.SetActive(false);
    }
}
