using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public InventoryItem myItem {  get; set; }
    public SlotTag myTag;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        // No carried item → nothing to place
        if (Inventory.carriedItem == null)
            return;

        // Slot tag restriction (equipment slots, etc.)
        if (myTag != SlotTag.None &&
            Inventory.carriedItem.myItem.itemTag != myTag)
            return;

        SetItem(Inventory.carriedItem);
    }
    public void SetItem(InventoryItem item)
    {
        if (item == null) return;

        InventorySlot fromSlot = item.activeSlot;
        InventoryItem itemInThisSlot = myItem;

        // STACKING
        if (itemInThisSlot != null &&
             itemInThisSlot != item &&
            itemInThisSlot.myItem.IsStackableItem() &&
            itemInThisSlot.myItem == item.myItem)
        {
            int total = itemInThisSlot.count + item.count;
            int maxStack = itemInThisSlot.myItem.GetMaxStackSize();

            if (total <= maxStack)
            {
                itemInThisSlot.AddStack(item.count);
                Destroy(item.gameObject);
                Inventory.carriedItem = null;
            }
            else
            {
                itemInThisSlot.count = maxStack;
                itemInThisSlot.UpdateCountText();
                item.count = total - maxStack;
                item.UpdateCountText();
            }
            return;
        }

        myItem = item;
        item.activeSlot = this;
        item.transform.SetParent(transform, false);
        item.canvasGroup.blocksRaycasts = true;

        RectTransform rt = item.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localScale = Vector3.one;

        // If item came from a slot
        if (fromSlot != null && fromSlot != this)
        {
            // Swap back the previous item (if any)
            fromSlot.myItem = itemInThisSlot;

            if (itemInThisSlot != null)
            {
                itemInThisSlot.activeSlot = fromSlot;
                itemInThisSlot.transform.SetParent(fromSlot.transform, false);
                itemInThisSlot.canvasGroup.blocksRaycasts = true;
            }
        }
        else if (fromSlot == null && itemInThisSlot != null && itemInThisSlot != item)
        {
            Inventory.carriedItem = itemInThisSlot;
            itemInThisSlot.activeSlot = null;
            itemInThisSlot.transform.SetParent(Inventory.Singleton.DraggableRoot, false);
            itemInThisSlot.canvasGroup.blocksRaycasts = false;
            return;
        }

        Inventory.carriedItem = null;

        if (myTag != SlotTag.None)
            Inventory.Singleton.EquipEquipment(myTag, myItem);

    }
    public void ClearSlot()
    {
        myItem = null;
    }
}
