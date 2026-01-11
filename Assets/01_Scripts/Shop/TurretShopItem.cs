using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TurretShopItem : MonoBehaviour
{
    [Header("Prefab de la torreta a comprar")]
    public GameObject turretPrefab;

    [Header("Inventario (padre donde se guardan las torretas)")]
    public Transform inventoryParent;

    [Header("Separación entre torretas en el inventario")]
    public float spacing = 0.3f;

    // Lista simple para llevar la cuenta de cuántas torretas compramos
    private static List<GameObject> ownedTurrets = new List<GameObject>();

    private XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        if (interactable == null)
        {
            Debug.LogError("TurretShopItem: falta XRSimpleInteractable en este objeto.");
        }
    }

    private void OnEnable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnSelected);
        }
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnSelected);
        }
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        // Aquí es donde "compras" la torreta
        BuyTurret();
    }

    private void BuyTurret()
    {
        if (turretPrefab == null || inventoryParent == null)
        {
            Debug.LogWarning("TurretShopItem: falta asignar turretPrefab o inventoryParent.");
            return;
        }

        // Calculamos una posición sencilla dentro del inventario:
        // vamos colocando cada torreta un poco más a la derecha
        int count = ownedTurrets.Count;
        Vector3 localPos = new Vector3(count * spacing, 0f, 0f);

        GameObject newTurret = Instantiate(
            turretPrefab,
            inventoryParent.TransformPoint(localPos),
            inventoryParent.rotation,
            inventoryParent // padre
        );

        ownedTurrets.Add(newTurret);
        Debug.Log("Compraste una torreta. Total en inventario: " + ownedTurrets.Count);
    }
}
