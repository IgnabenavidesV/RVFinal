using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class BuildSpot : MonoBehaviour
{
    public Transform mountPoint;
    public BuildManager buildManager;

    private XRSimpleInteractable interactable;
    private GameObject placedTurret;

    public bool IsOccupied => placedTurret != null;

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
        if (buildManager != null)
            buildManager.OnSpotClicked(this);
    }

    public void PlaceTurret(GameObject turretPrefab)
    {
        if (IsOccupied || turretPrefab == null || mountPoint == null) return;

        placedTurret = Instantiate(turretPrefab, mountPoint.position, mountPoint.rotation);

        // (opcional) si quieres que quede exactamente como hijo del spot:
        // placedTurret.transform.SetParent(mountPoint, true);
    }
}
