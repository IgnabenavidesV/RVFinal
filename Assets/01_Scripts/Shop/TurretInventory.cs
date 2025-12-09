using UnityEngine;

public class TurretInventory : MonoBehaviour
{
    [Header("Slots donde aparecerán las torretas")]
    public Transform[] slots;

    private GameObject[] turrets;

    private void Awake()
    {
        turrets = new GameObject[slots.Length];
    }

    public bool AddTurret(GameObject turretPrefab)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (turrets[i] == null)
            {
                GameObject t = Instantiate(
                    turretPrefab,
                    slots[i].position,
                    slots[i].rotation,
                    slots[i]
                );

                turrets[i] = t;
                return true;
            }
        }

        Debug.Log("Inventario lleno");
        return false;
    }
}
