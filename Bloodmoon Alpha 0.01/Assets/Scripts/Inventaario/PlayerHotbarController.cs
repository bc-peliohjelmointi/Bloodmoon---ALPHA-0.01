using UnityEngine;

public class PlayerHotbarController : MonoBehaviour
{
    public static PlayerHotbarController Instance;

    [Header("References")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform handSocket; // where axe appears (camera child)

    [Header("Settings")]
    public int selectedIndex = 0;

    private GameObject currentEquippedGO;

    private void Awake()
    {
        Instance = this;
        inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
    }

    private void Update()
    {
        HandleScroll();
        HandleNumberKeys();
    }

    void HandleScroll()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (scroll == 0) return;

        if (scroll > 0)
            selectedIndex--;
        else
            selectedIndex++;

        selectedIndex = Mathf.Clamp(
            selectedIndex,
            0,
            inventory.HotbarCount - 1
        );

        EquipSelectedSlot();
    }

    void HandleNumberKeys()
    {
        for (int i = 0; i < inventory.HotbarCount; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                selectedIndex = i;
                EquipSelectedSlot();
            }
        }
    }

    void EquipSelectedSlot()
    {
        InventorySlot slot = inventory.GetHotbarSlot(selectedIndex);

        UnequipCurrent();

        if (slot == null || slot.myItem == null)
            return;

        Item item = slot.myItem.myItem;

        if (item.equipmentPrefab != null)
        {
            currentEquippedGO = Instantiate(
                item.equipmentPrefab,
                handSocket
            );
        }
    }

    void UnequipCurrent()
    {
        if (currentEquippedGO != null)
            Destroy(currentEquippedGO);
    }

    public Item GetEquippedItem()
    {
        InventorySlot slot = inventory.GetHotbarSlot(selectedIndex);
        return slot?.myItem?.myItem;
    }
}