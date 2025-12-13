using System.Collections.Generic;
using UnityEngine;

public class VRInventory : MonoBehaviour
{
    [Header("Slots visuales")]
    public Transform[] slots;

    private List<GameObject> items = new List<GameObject>();
    private int selectedIndex = 0;

    [Header("Visual")]
    public float selectedScale = 1.3f;
    public float normalScale = 1f;

    private void Start()
    {
        UpdateVisuals();
    }

    public void AddItem(GameObject itemPrefab)
    {
        if (items.Count >= slots.Length)
        {
            Debug.Log("Inventario lleno");
            return;
        }

        GameObject item = Instantiate(
            itemPrefab,
            slots[items.Count].position,
            slots[items.Count].rotation,
            slots[items.Count]
        );

        item.transform.localScale *= 0.4f; // miniatura
        items.Add(item);

        UpdateVisuals();
    }

    public GameObject GetSelectedItem()
    {
        if (items.Count == 0) return null;
        return items[selectedIndex];
    }

    public void SelectNext()
    {
        if (items.Count == 0) return;
        selectedIndex = (selectedIndex + 1) % items.Count;
        UpdateVisuals();
    }

    public void SelectPrevious()
    {
        if (items.Count == 0) return;
        selectedIndex--;
        if (selectedIndex < 0) selectedIndex = items.Count - 1;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].transform.localScale =
                Vector3.one * (i == selectedIndex ? selectedScale : normalScale) * 0.4f;
        }
    }
}
