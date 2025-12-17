using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TurretShopItem : MonoBehaviour
{
    [Header("Item")]
    public GameObject turretPrefab;
    public VRInventory inventory;

    [Header("Precio (fijo por torre)")]
    [Range(200, 500)]
    public int price = 200;

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

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance es NULL. ¿Hay un GameManager en la escena?");
            return;
        }

        // ✅ Cobrar antes de entregar
        bool paid = GameManager.Instance.SpendMoney(price);
        if (!paid)
        {
            Debug.Log($"No alcanza el dinero. Precio: {price}, Dinero: {GameManager.Instance.money}");
            // aquí puedes reproducir un sonido / feedback UI
            return;
        }

        // ✅ Comprar: recién ahora se da el item
        inventory.AddItem(turretPrefab);
        Debug.Log($"Comprado: {turretPrefab.name} por {price}. Dinero restante: {GameManager.Instance.money}");
    }
}
