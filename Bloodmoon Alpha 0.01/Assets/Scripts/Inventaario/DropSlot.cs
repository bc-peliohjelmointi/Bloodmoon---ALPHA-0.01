using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : InventorySlot
{
    [Header("Drop Settings")]
    [SerializeField] private Transform dropPoint; // where item spawns (e.g. in front of player)
    [SerializeField] private float dropForce = 2f;

    public override void SetItem(InventoryItem item)
    {
        if (item == null || item.myItem == null) return;

        // Drop all of the stack
        int totalCount = item.count;

        for (int i = 0; i < totalCount; i++)
        {
            DropItem(item);
        }

        // Destroy the inventory UI item
        Destroy(item.gameObject);

        // Clear carried item and slot
        Inventory.carriedItem = null;
        myItem = null;
    }

    void DropItem(InventoryItem item)
    {
        if (item.myItem.worldPrefab == null)
        {
            Debug.LogWarning($"No world prefab for {item.myItem.name}");
            return;
        }

        Vector3 spawnPos = dropPoint != null
            ? dropPoint.position
            : Camera.main.transform.position + Camera.main.transform.forward * 2f;

        GameObject dropped = Instantiate(item.myItem.worldPrefab, spawnPos, Quaternion.identity);

        // Optional: add a little force so it "pops" out
        Rigidbody rb = dropped.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 dir = Camera.main.transform.forward;
            rb.AddForce(dir * dropForce, ForceMode.Impulse);
        }

        // Assign item data to pickup script
        WorldItemPickup pickup = dropped.GetComponent<WorldItemPickup>();
        if (pickup != null)
        {
            pickup.item = item.myItem;
        }
    }
}