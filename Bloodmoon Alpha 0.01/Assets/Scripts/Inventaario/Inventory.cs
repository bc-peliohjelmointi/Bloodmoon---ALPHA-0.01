using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory Singleton;
    public static InventoryItem carriedItem;

    [SerializeField] InventorySlot[] inventorySlots;
    [SerializeField] InventorySlot[] hotbarSlots;

    [SerializeField] InventorySlot[] equipmentSlots;

    [SerializeField] Transform draggablesTransform;
    public Transform DraggableRoot => draggablesTransform;
    [SerializeField] InventoryItem itemPrefab;

    [Header("Item List")]
    [SerializeField] Item[] items;

    [Header("Debug")]
    [SerializeField] Button giveItemBtn;


    private void Awake()
    {
        Singleton = this;
        giveItemBtn.onClick.AddListener(() => SpawnInventoryItem());
    }

    private void Update()
    {
        if (carriedItem != null)
            carriedItem.transform.position = Input.mousePosition;
    }

    public void SetCarriedItem(InventoryItem item)
    {
        if (carriedItem != null) return;

        InventorySlot fromSlot = item.activeSlot;

        if (fromSlot != null)
        {
            fromSlot.ClearSlot();
        }

        carriedItem = item;
        carriedItem.canvasGroup.blocksRaycasts = false;
        item.transform.SetParent(draggablesTransform);
    }

    public void EquipEquipment(SlotTag tag, InventoryItem item = null)
    {
        switch (tag)
        {
            case SlotTag.Head:
                if (item == null)
                    Debug.Log("Unequipped helmet on " + tag);
                else
                    Debug.Log("Equipped " + item.myItem.name + " on " + tag);
                break;
            case SlotTag.Chest: break;
            case SlotTag.Legs: break;
            case SlotTag.Feet: break;
        }
    }
    public bool CanCraft(Recipe recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            int totalFound = 0;

            foreach (InventorySlot slot in inventorySlots)
            {
                if (slot.myItem != null && slot.myItem.myItem == ingredient.item)
                {
                    totalFound += slot.myItem.count;
                }
            }

            if (totalFound < ingredient.amount)
                return false;
        }

        return true;
    }
    public void Craft(Recipe recipe)
    {
        if (!CanCraft(recipe))
        {
            Debug.Log("Not enough materials!");
            return;
        }

        // REMOVE INGREDIENTS
        foreach (var ingredient in recipe.ingredients)
        {
            int amountToRemove = ingredient.amount;

            foreach (InventorySlot slot in inventorySlots)
            {
                if (slot.myItem == null) continue;
                if (slot.myItem.myItem != ingredient.item) continue;

                int removeFromThisSlot = Mathf.Min(amountToRemove, slot.myItem.count);

                slot.myItem.count -= removeFromThisSlot;
                slot.myItem.UpdateCountText();

                amountToRemove -= removeFromThisSlot;

                if (slot.myItem.count <= 0)
                {
                    Destroy(slot.myItem.gameObject);
                    slot.ClearSlot();
                }

                if (amountToRemove <= 0)
                    break;
            }
        }

        // SPAWN RESULT
        for (int i = 0; i < recipe.resultAmount; i++)
        {
            SpawnInventoryItem(recipe.result);
        }
    }

    public void SpawnInventoryItem(Item item = null)
    {
        Item _item = item ?? PickRandomItem();

        // Merge stackable items if possible
        if (_item.itemTag == SlotTag.Stackable)
        {
            foreach (InventorySlot slot in inventorySlots)
            {
                if (slot.myItem != null && slot.myItem.myItem == _item)
                {
                    int maxStack = 64;
                    int spaceLeft = maxStack - slot.myItem.count;

                    if (spaceLeft > 0)
                    {
                        slot.myItem.AddStack(1);
                        return; // merged successfully
                    }
                }
            }
        }

        // Otherwise, spawn in an empty slot
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].myItem == null)
            {
                InventoryItem newItem = Instantiate(itemPrefab, inventorySlots[i].transform);

                // Reset RectTransform to fill the slot
                RectTransform rt = newItem.GetComponent<RectTransform>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                rt.localScale = Vector3.one;

                newItem.Initialize(_item, inventorySlots[i]);

                // Place the item into the slot properly
                inventorySlots[i].SetItem(newItem);

                break;
            }
        }
    }

    private Item PickRandomItem()
    {
        int random = Random.Range(0, items.Length);
        return items[random];
    }
    public int HotbarCount => hotbarSlots.Length;

    public InventorySlot GetHotbarSlot(int index)
    {
        if (index < 0 || index >= hotbarSlots.Length)
            return null;

        return hotbarSlots[index];
    }
    public bool ConsumeItem(Item item, int amount)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.myItem == null) continue;
            if (slot.myItem.myItem != item) continue;

            int remove = Mathf.Min(amount, slot.myItem.count);
            slot.myItem.count -= remove;
            slot.myItem.UpdateCountText();

            amount -= remove;

            if (slot.myItem.count <= 0)
            {
                Destroy(slot.myItem.gameObject);
                slot.ClearSlot();
            }

            if (amount <= 0)
                return true;
        }

        return false;
    }
}