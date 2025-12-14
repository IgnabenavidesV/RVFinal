using UnityEngine;
using UnityEngine.InputSystem;

public class BuildManager : MonoBehaviour
{
    public VRInventory inventory;

    [Header("Cancelar preview")]
    public InputActionProperty cancelAction; // usa XRI Right Interaction/Activate por ejemplo

    private BuildSpot pendingSpot;
    private GameObject preview;
    private GameObject pendingPrefab;

    [Header("Preview Float Animation")]
    public float floatAmplitude = 0.01f; // 1 cm
    public float floatSpeed = 2f;
    public float previewFloatOffset = 0.08f;

    private Vector3 previewBasePos;

    private void Update()
    {
        if (preview != null)
        {
            float offset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            preview.transform.position = previewBasePos + Vector3.up * offset;
        }
    }

    private void OnEnable()
    {
        if (cancelAction.action != null)
        {
            cancelAction.action.Enable();
            cancelAction.action.performed += OnCancel;
        }
    }

    private void OnDisable()
    {
        if (cancelAction.action != null)
        {
            cancelAction.action.performed -= OnCancel;
            cancelAction.action.Disable();
        }
    }

    public void OnSpotClicked(BuildSpot spot)
    {
        if (spot == null || inventory == null) return;
        if (spot.IsOccupied) return;

        var prefab = inventory.GetSelectedPrefab();
        if (prefab == null)
        {
            Debug.Log("No hay torreta seleccionada.");
            CancelBuild();
            return;
        }

        // ? Si clickeas el MISMO spot teniendo preview => CONFIRMAR
        if (pendingSpot == spot && pendingPrefab == prefab && preview != null)
        {
            ConfirmPlacement();
            return;
        }

        // ? Nuevo spot o cambiaste de torreta => preview
        pendingSpot = spot;
        pendingPrefab = prefab;
        ShowPreview(prefab, spot);
    }

    private void ShowPreview(GameObject prefab, BuildSpot spot)
    {
        ClearPreviewOnly();

        preview = Instantiate(prefab, spot.mountPoint.position, spot.mountPoint.rotation);

        previewBasePos = spot.mountPoint.position + Vector3.up * previewFloatOffset;
        preview.transform.position = previewBasePos;

        MakePreview(preview);
    }

    private void ConfirmPlacement()
    {
        if (pendingSpot == null || pendingPrefab == null) return;

        // ? Consumir del inventario
        if (!inventory.ConsumeSelected())
        {
            CancelBuild();
            return;
        }

        // ? Colocar torreta fija
        pendingSpot.PlaceTurret(pendingPrefab);

        // ? Limpiar preview + estado
        CancelBuild();
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        CancelBuild();
    }

    private void ClearPreviewOnly()
    {
        if (preview != null) Destroy(preview);
        preview = null;
    }

    public void CancelBuild()
    {
        ClearPreviewOnly();
        pendingSpot = null;
        pendingPrefab = null;
    }

    private void MakePreview(GameObject go)
    {
        foreach (var c in go.GetComponentsInChildren<Collider>(true))
            c.enabled = false;

        foreach (var rb in go.GetComponentsInChildren<Rigidbody>(true))
            rb.isKinematic = true;

        foreach (var mb in go.GetComponentsInChildren<MonoBehaviour>(true))
            mb.enabled = false;

        int ignore = LayerMask.NameToLayer("Ignore Raycast");
        foreach (Transform t in go.GetComponentsInChildren<Transform>(true))
            t.gameObject.layer = ignore;
    }
}
