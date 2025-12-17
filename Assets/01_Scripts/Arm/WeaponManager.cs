using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    public Transform weaponHolder;
    public VRInventory inventory;

    private GameObject equippedWeapon;
    private GameObject pickupObject; // pedestal/pickup

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

        // (opcional) bloquear cosas mientras tienes arma equipada
        if (inventory != null) inventory.gameObject.SetActive(false);
        if (inventoryInput != null) inventoryInput.enabled = false;
        if (buildManager != null) buildManager.enabled = false;

        equippedWeapon = Instantiate(weaponPrefab, weaponHolder);

        equippedWeapon.transform.localPosition = equippedLocalPosition;
        equippedWeapon.transform.localEulerAngles = equippedLocalRotation;
        equippedWeapon.transform.localScale = equippedLocalScale;

        var bz = equippedWeapon.GetComponentInChildren<BazookaWeapon>(true);
        if (bz != null) bz.SetEquipped(true);
    }

    public void UnequipWeapon()
    {
        if (equippedWeapon == null) return;

        // ? primero apaga la lógica del arma (antes de destruir)
        var bz = equippedWeapon.GetComponentInChildren<BazookaWeapon>(true);
        if (bz != null) bz.SetEquipped(false);

        Destroy(equippedWeapon);
        equippedWeapon = null;

        // ? reactivar sistemas
        if (inventory != null) inventory.gameObject.SetActive(true);
        if (inventoryInput != null) inventoryInput.enabled = true;
        if (buildManager != null) buildManager.enabled = true;

        // ? devolver pedestal: al activarse, BazookaPickup.OnEnable resetea picked=false
        if (pickupObject != null) pickupObject.SetActive(true);
        pickupObject = null;
    }
}
