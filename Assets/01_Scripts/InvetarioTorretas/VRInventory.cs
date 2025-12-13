using System.Collections.Generic;
using UnityEngine;

public class VRInventory : MonoBehaviour
{
    [Header("Slots visuales")]
    public Transform[] slots;

    [Header("Visual")]
    public float selectedScale = 1.3f;
    public float normalScale = 1f;
    public float iconScale = 0.4f;

    private class Entry
    {
        public GameObject prefab; // el prefab real a colocar
        public GameObject icon;   // el “icono” 3D (instancia visual)
    }

    private readonly List<Entry> items = new();
    private int selectedIndex = 0;

    public int Count => items.Count;
    public bool HasItems => items.Count > 0;

    private void Start() => UpdateVisuals();

    // ----------------------------
    // API pública
    // ----------------------------

    public void AddItem(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning("VRInventory.AddItem: prefab null");
            return;
        }

        if (slots == null || slots.Length == 0)
        {
            Debug.LogError("VRInventory: No tienes slots asignados.");
            return;
        }

        if (items.Count >= slots.Length)
        {
            Debug.Log("Inventario lleno");
            return;
        }

        var slot = slots[items.Count];

        var icon = Instantiate(prefab, slot.position, slot.rotation, slot);
        icon.transform.localScale = Vector3.one * iconScale;

        MakeIcon(icon);

        items.Add(new Entry { prefab = prefab, icon = icon });

        if (items.Count == 1)
            selectedIndex = 0;

        UpdateVisuals();
    }

    public GameObject GetSelectedPrefab()
    {
        if (!HasItems) return null;
        selectedIndex = Mathf.Clamp(selectedIndex, 0, items.Count - 1);
        return items[selectedIndex].prefab;
    }

    public bool TryGetSelectedPrefab(out GameObject prefab)
    {
        prefab = GetSelectedPrefab();
        return prefab != null;
    }

    // Consume el ítem seleccionado (se usa al CONFIRMAR placement)
    public bool ConsumeSelected()
    {
        if (!HasItems) return false;

        selectedIndex = Mathf.Clamp(selectedIndex, 0, items.Count - 1);

        // borrar icono visual
        if (items[selectedIndex].icon != null)
            Destroy(items[selectedIndex].icon);

        items.RemoveAt(selectedIndex);

        // asegurar índice válido
        if (items.Count == 0)
        {
            selectedIndex = 0;
            // limpiar slots visuales por si algo quedó
            ClearAllSlots();
            return true;
        }

        if (selectedIndex >= items.Count)
            selectedIndex = items.Count - 1;

        RebuildIcons();
        UpdateVisuals();
        return true;
    }

    public void SelectNext()
    {
        if (!HasItems) return;
        selectedIndex = (selectedIndex + 1) % items.Count;
        UpdateVisuals();
    }

    public void SelectPrevious()
    {
        if (!HasItems) return;
        selectedIndex = (selectedIndex - 1 + items.Count) % items.Count;
        UpdateVisuals();
    }

    // ----------------------------
    // Internals (visual / icon mode)
    // ----------------------------

    private void RebuildIcons()
    {
        // Reacomoda los iconos en slots desde 0..n-1
        for (int i = 0; i < items.Count; i++)
        {
            var slot = slots[i];

            // Si por alguna razón falta el icono, lo recreamos
            if (items[i].icon == null)
            {
                items[i].icon = Instantiate(items[i].prefab, slot.position, slot.rotation, slot);
                items[i].icon.transform.localScale = Vector3.one * iconScale;
                MakeIcon(items[i].icon); // ? importantísimo
            }

            // Asegurar parent y transform
            items[i].icon.transform.SetParent(slot, true);
            items[i].icon.transform.position = slot.position;
            items[i].icon.transform.rotation = slot.rotation;
        }

        // Limpia slots restantes (por si quedó algo)
        for (int i = items.Count; i < slots.Length; i++)
        {
            for (int c = slots[i].childCount - 1; c >= 0; c--)
                Destroy(slots[i].GetChild(c).gameObject);
        }
    }

    private void UpdateVisuals()
    {
        for (int i = 0; i < items.Count; i++)
        {
            float target = (i == selectedIndex) ? selectedScale : normalScale;
            if (items[i].icon != null)
                items[i].icon.transform.localScale = Vector3.one * iconScale * target;
        }
    }

    private void ClearAllSlots()
    {
        if (slots == null) return;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;
            for (int c = slots[i].childCount - 1; c >= 0; c--)
                Destroy(slots[i].GetChild(c).gameObject);
        }
    }

    private void MakeIcon(GameObject go)
    {
        DisableGameplay(go);
        SetLayerRecursively(go, LayerMask.NameToLayer("Ignore Raycast"));
    }

    private void DisableGameplay(GameObject go)
    {
        // Colliders fuera (para que no interactúe ni dispare por triggers)
        foreach (var c in go.GetComponentsInChildren<Collider>(true))
            c.enabled = false;

        // Rigidbodies fuera (por si tu torreta usa físicas)
        foreach (var rb in go.GetComponentsInChildren<Rigidbody>(true))
            rb.isKinematic = true;

        // Desactivar TODOS los scripts (MonoBehaviour) del prefab
        foreach (var mb in go.GetComponentsInChildren<MonoBehaviour>(true))
            mb.enabled = false;
    }

    private void SetLayerRecursively(GameObject go, int layer)
    {
        if (layer < 0) return; // por si Ignore Raycast no existe (aunque Unity lo trae)
        foreach (Transform t in go.GetComponentsInChildren<Transform>(true))
            t.gameObject.layer = layer;
    }
}
