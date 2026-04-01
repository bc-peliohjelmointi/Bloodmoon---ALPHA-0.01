using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public InventoryItem myItem { get; set; }
    public SlotTag myTag;

    // Make this method virtual
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (Inventory.carriedItem == null) return;
        if (myTag != SlotTag.None && Inventory.carriedItem.myItem.itemTag != myTag) return;

        SetItem(Inventory.carriedItem);
    }

    // Make this method virtual
    public virtual void SetItem(InventoryItem item)
    {
        if (item == null) return;

        InventoryItem itemInSlot = myItem;
        InventorySlot fromSlot = item.activeSlot;

        if (itemInSlot == null)
        {
            PlaceItemInSlot(item);
            if (fromSlot != null) fromSlot.myItem = null;
            Inventory.carriedItem = null;
            return;
        }

        if (itemInSlot.myItem.IsStackableItem() && item.myItem.IsStackableItem() && itemInSlot.myItem == item.myItem)
        {
            int maxStack = 100;
            int spaceLeft = maxStack - itemInSlot.count;
            if (spaceLeft > 0)
            {
                int amountToMove = Mathf.Min(spaceLeft, item.count);
                itemInSlot.AddStack(amountToMove);
                item.count -= amountToMove;
                item.UpdateCountText();
            }
            return;
        }

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
    }

    protected void PlaceItemInSlot(InventoryItem item)
    {
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
    }

    public void ClearSlot()
    {
        myItem = null;
    }
}