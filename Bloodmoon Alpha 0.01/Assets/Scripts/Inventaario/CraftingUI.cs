using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CraftingUI : MonoBehaviour
{
    public static CraftingUI Instance;

    [Header("References")]
    [SerializeField] private Transform contentParent;

    private readonly List<CraftingRecipeSlot> slots = new();

    private void Awake() => Instance = this;

    private void Start()
    {
        foreach (Transform child in contentParent)
        {
            if (child.TryGetComponent(out CraftingRecipeSlot slot))
                slots.Add(slot);
        }

        Debug.Log($"[CraftingUI] Found {slots.Count} recipe slots.");

        if (Inventory.Singleton == null)
        {
            Debug.LogError("[CraftingUI] Inventory.Singleton is null!");
            return;
        }

        foreach (var slot in slots)
            Debug.Log($"  Slot: {slot.recipe?.name} | CanCraft: {Inventory.Singleton.CanCraft(slot.recipe)}");

        Inventory.Singleton.InventorySlotChanged += _ => RefreshAll();
        RefreshAll();
    }

    private void OnDestroy()
    {
        if (Inventory.Singleton != null)
            Inventory.Singleton.InventorySlotChanged -= _ => RefreshAll();
    }

    public void RefreshAll()
    {
        foreach (var slot in slots)
            slot.Refresh();

        // Sort: craftable recipes bubble to the top
        var sorted = slots
            .OrderByDescending(s => Inventory.Singleton.CanCraft(s.recipe))
            .ToList();

        for (int i = 0; i < sorted.Count; i++)
            sorted[i].transform.SetSiblingIndex(i);
    }
}