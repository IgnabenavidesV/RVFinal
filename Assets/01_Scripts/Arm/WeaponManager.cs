using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    public Transform weaponHolder;
    public VRInventory inventory;

    private GameObject equippedWeapon;
    private GameObject pickupObject; // <- referencia al pedestal/pickup

    [Header("Equip Offsets")]
    public Vector3 equippedLocalPosition = new Vector3(0f, -0.05f, 0.15f);
    public Vector3 equippedLocalRotation = new Vector3(0f, 0f, 0f);
    public Vector3 equippedLocalScale = Vector3.one * 0.3f;
    public MonoBehaviour inventoryInput;  // tu script InventoryInput
    public MonoBehaviour buildManager;    // tu BuildManager

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool IsWeaponEquipped => equippedWeapon != null;

    public void EquipWeapon(GameObject weaponPrefab, GameObject pickupSource)
    {
        if (equippedWeapon != null) return;

        pickupObject = pickupSource;

        if (inventory != null)
            inventory.gameObject.SetActive(false);

        // ?? AQUÍ se instancia el prefab
        equippedWeapon = Instantiate(weaponPrefab, weaponHolder);

        equippedWeapon.transform.localPosition = equippedLocalPosition;
        equippedWeapon.transform.localEulerAngles = equippedLocalRotation;
        equippedWeapon.transform.localScale = equippedLocalScale;

        // activar lógica del arma
        var bz = equippedWeapon.GetComponentInChildren<BazookaWeapon>(true);
        if (bz != null)
            bz.SetEquipped(true);
    }
    public void UnequipWeapon()
    {
        if (equippedWeapon == null) return;

        Destroy(equippedWeapon);
        equippedWeapon = null;

        if (inventory != null)
            inventory.gameObject.SetActive(true);

        if (inventoryInput != null)
            inventoryInput.enabled = true;

        if (buildManager != null)
            buildManager.enabled = true;

        if (pickupObject != null)
            StartCoroutine(ReenablePickupNextFrame(pickupObject));

        pickupObject = null;
    }

    private System.Collections.IEnumerator ReenablePickupNextFrame(GameObject obj)
    {
        yield return null; // ? esperar 1 frame
        if (obj != null) obj.SetActive(true);
    }
}
