using UnityEngine;
using UnityEngine.EventSystems;

public class TrashSlot : InventorySlot
{
    public override void SetItem(InventoryItem item)
    {
        if (item == null) return;

        // Destroy the item immediately
        Destroy(item.gameObject);

        // Clear slot and carried reference
        myItem = null;
        if (Inventory.carriedItem == item) Inventory.carriedItem = null;

        Debug.Log("Item trashed!");
    }

}