using UnityEngine;
using UnityEngine.InputSystem;

public class BuildManager : MonoBehaviour
{
    public VRInventory inventory;

    [Header("Cancelar preview")]
    public InputActionProperty cancelAction; // asigna B/Y o SecondaryButton

    private BuildSpot pendingSpot;
    private GameObject preview;
    private GameObject pendingPrefab;

    [Header("Preview Float Animation")]
    public float floatAmplitude = 0.01f; // 1 cm
    public float floatSpeed = 2f;

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
            CancelPreview();
            Debug.Log("No hay torreta seleccionada.");
            return;
        }

        // Segundo click en el MISMO spot = confirmar
        if (pendingSpot == spot && pendingPrefab == prefab)
        {
            ConfirmPlacement();
            return;
        }

        // Primer click o cambiaste spot/prefab = preview
        pendingSpot = spot;
        pendingPrefab = prefab;
        ShowPreview(prefab, spot);
    }

    private void ShowPreview(GameObject prefab, BuildSpot spot)
    {
        CancelPreview();

        preview = Instantiate(prefab, spot.mountPoint.position, spot.mountPoint.rotation);

        // Base de la animación (un poco arriba del punto real)
        previewBasePos = spot.mountPoint.position + Vector3.up * 0.08f;
        preview.transform.position = previewBasePos;

        MakePreview(preview); 
    }


    private void ConfirmPlacement()
    {
        if (pendingSpot == null || pendingPrefab == null) return;

        // consumir del inventario (desaparece)
        if (!inventory.ConsumeSelected())
        {
            CancelPreview();
            return;
        }

        // colocar real
        pendingSpot.PlaceTurret(pendingPrefab);

        CancelPreview();
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        CancelPreview();
    }

    public void CancelPreview()
    {
        if (preview != null) Destroy(preview);
        preview = null;
        pendingSpot = null;
        pendingPrefab = null;
    }

    private void MakePreview(GameObject go)
    {
        // Desactiva gameplay/colliders
        foreach (var c in go.GetComponentsInChildren<Collider>(true))
            c.enabled = false;

        foreach (var rb in go.GetComponentsInChildren<Rigidbody>(true))
            rb.isKinematic = true;

        foreach (var mb in go.GetComponentsInChildren<MonoBehaviour>(true))
            mb.enabled = false;

        // Preview no debe bloquear ray
        int ignore = LayerMask.NameToLayer("Ignore Raycast");
        foreach (Transform t in go.GetComponentsInChildren<Transform>(true))
            t.gameObject.layer = ignore;

        // (Opcional) si quieres que el preview sea “fantasma”, aquí podrías cambiar materiales,
        // pero lo dejamos simple.
    }
}
