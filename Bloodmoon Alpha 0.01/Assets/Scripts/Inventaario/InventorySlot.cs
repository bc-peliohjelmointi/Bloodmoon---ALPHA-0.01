using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public InventoryItem myItem { get; set; }
    public SlotTag myTag;

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (Inventory.carriedItem == null) return;
        if (myTag != SlotTag.None && Inventory.carriedItem.myItem.itemTag != myTag) return;

        SetItem(Inventory.carriedItem);
    }

    public virtual void SetItem(InventoryItem item)
    {
        if (item == null) return;

        InventorySlot fromSlot = item.activeSlot;
        InventoryItem itemInSlot = myItem;

        // Case: dropping into the same slot
        if (fromSlot == this)
        {
            // Just reset the item's position in the slot
            PlaceItemInSlot(item);
            Inventory.carriedItem = null;
            return;
        }

        // Case: slot empty
        if (itemInSlot == null)
        {
            PlaceItemInSlot(item);
            if (fromSlot != null) fromSlot.myItem = null;
            Inventory.carriedItem = null;
            return;
        }

        // Case: stackable merge
        if (itemInSlot.myItem.IsStackableItem() && item.myItem.IsStackableItem() && itemInSlot.myItem == item.myItem)
        {
            int maxStack = itemInSlot.myItem.GetMaxStackSize();
            int spaceLeft = maxStack - itemInSlot.count;
            if (spaceLeft > 0)
            {
                int amountToMove = Mathf.Min(spaceLeft, item.count);
                itemInSlot.AddStack(amountToMove);
                item.count -= amountToMove;
                item.UpdateCountText();

                if (item.count <= 0)
                {
                    if (fromSlot != null) fromSlot.myItem = null;
                    Destroy(item.gameObject);
                    Inventory.carriedItem = null;
                }
            }
            return;
        }

        // Case: swap items
        PlaceItemInSlot(item);
        if (fromSlot != null)
        {
            fromSlot.myItem = itemInSlot;
            if (itemInSlot != null)
            {
                itemInSlot.activeSlot = fromSlot;
                itemInSlot.transform.SetParent(fromSlot.transform, false);
                itemInSlot.canvasGroup.blocksRaycasts = true;
            }
        }

        Inventory.carriedItem = null;
        PlayerHotbarController.Instance?.OnSlotUpdated(this);
    }


    protected void PlaceItemInSlot(InventoryItem item)
    {
        myItem = item;
        item.activeSlot = this;
        item.transform.SetParent(transform, false);
        if (item.canvasGroup == null)
        {
            item.canvasGroup = GetComponentInChildren<CanvasGroup>();
        }
        item.canvasGroup.blocksRaycasts = true;

        RectTransform rt = item.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localScale = Vector3.one;

        PlayerHotbarController.Instance?.OnSlotUpdated(this);
        Inventory.Singleton?.UpdateSlot(this); // ADD THIS
    }

    public void ClearSlot()
    {
        myItem = null;
        PlayerHotbarController.Instance?.OnSlotUpdated(this);
        Inventory.Singleton?.UpdateSlot(this); // ADD THIS
    }

}