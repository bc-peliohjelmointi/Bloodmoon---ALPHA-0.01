using UnityEngine;

public class PlayerHotbarController : MonoBehaviour
{
    public static PlayerHotbarController Instance;

    [Header("References")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform handSocket; // where axe appears (camera child)

    [Header("Settings")]
    public int selectedIndex = 0;
    [SerializeField] private KeyCode useConsumableKey = KeyCode.E;

    private GameObject currentEquippedGO;

    private void Awake()
    {
        Instance = this;
        inventory = GameObject.Find("Inventory").GetComponent<Inventory>();

        // Subscribe to slot change events
        inventory.InventorySlotChanged += OnInventorySlotChanged;
    }

    private void OnInventorySlotChanged(InventorySlot slot)
    {
        // If the changed slot is the currently selected hotbar slot, re-equip
        int currentIndex = selectedIndex;
        InventorySlot currentSlot = inventory.GetHotbarSlot(currentIndex);

        if (slot == currentSlot)
        {
            EquipSelectedSlot();
        }
    }

    private void Update()
    {
        HandleScroll();
        HandleNumberKeys();
        TryUseSelectedConsumable();
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

        // Always unequip current first
        UnequipCurrent();

        // If slot is empty, just leave nothing equipped
        if (slot == null || slot.myItem == null)
            return;

        Item item = slot.myItem.myItem;

        if (item.equipmentPrefab != null)
        {
            currentEquippedGO = Instantiate(item.equipmentPrefab, handSocket);
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

    void TryUseSelectedConsumable()
    {
        if (!Input.GetKeyDown(useConsumableKey))
            return;

        InventorySlot slot = inventory.GetHotbarSlot(selectedIndex);
        if (slot == null || slot.myItem == null || slot.myItem.myItem == null)
            return;

        Item item = slot.myItem.myItem;
        bool canConsume = item.itemTag == SlotTag.Food || item.itemTag == SlotTag.Water;

        if (!canConsume)
            return;

        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        if (playerController == null)
        {
            Debug.LogWarning("PlayerController reference missing for consumables.");
            return;
        }

        if (item.itemTag == SlotTag.Food)
            playerController.RestoreFood(10);

        if (item.itemTag == SlotTag.Water)
            playerController.RestoreWater(10);

        inventory.ConsumeFromSlot(slot, 1);
        EquipSelectedSlot();
    }
    public void OnSlotUpdated(InventorySlot slot)
    {
        // If the slot that changed is the currently selected hotbar slot
        if (inventory.GetHotbarSlot(selectedIndex) == slot)
        {
            // Unequip immediately
            UnequipCurrent();

            // Equip again only if there's an item
            InventorySlot currentSlot = inventory.GetHotbarSlot(selectedIndex);
            if (currentSlot != null && currentSlot.myItem != null)
            {
                Item item = currentSlot.myItem.myItem;
                if (item.equipmentPrefab != null)
                    currentEquippedGO = Instantiate(item.equipmentPrefab, handSocket);
            }
        }
    }
}
