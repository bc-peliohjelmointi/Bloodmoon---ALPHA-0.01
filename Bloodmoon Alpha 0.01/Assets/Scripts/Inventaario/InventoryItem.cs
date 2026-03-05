using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    [Header("UI References")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text countText;

    public CanvasGroup canvasGroup { get; private set; }

    public Item myItem { get; set; }
    public InventorySlot activeSlot { get; set; }

    [HideInInspector]
    public int count = 1;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Initialize(Item item, InventorySlot parent)
    {
        activeSlot = parent;

        if (activeSlot != null)
            activeSlot.myItem = this;

        myItem = item;

        if (itemIcon != null && myItem != null)
            itemIcon.sprite = myItem.sprite;

        if (myItem.IsStackableItem())
        {
            count = 1;
            UpdateCountText();
        }
        else
        {
            if (countText != null)
                countText.text = "";
        }
    }

    public void AddStack(int amount)
    {
        int maxStack = myItem != null ? myItem.GetMaxStackSize() : int.MaxValue;
        count = Mathf.Clamp(count + amount, 0, maxStack);
        UpdateCountText();
    }

    public void UpdateCountText()
    {
        if (countText != null)
            countText.text = count > 1 ? count.ToString() : "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Inventory.Singleton.SetCarriedItem(this);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            TrySplitStack();
        }
    }

    private void TrySplitStack()
    {
        if (!myItem.IsStackableItem()) return;
        if (count <= 1) return;

        int half = Mathf.CeilToInt(count / 2f);

        count -= half;
        UpdateCountText();

        InventoryItem splitItem =
            Instantiate(this, Inventory.Singleton.DraggableRoot);

        splitItem.myItem = this.myItem;

        splitItem.activeSlot = null;
        splitItem.count = half;
        splitItem.UpdateCountText();

        Inventory.carriedItem = splitItem;
        splitItem.canvasGroup.blocksRaycasts = false;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (activeSlot != null)
        {
            activeSlot.SetItem(this);
            // activeSlot will now reference this item, no problem
        }
    }
}
