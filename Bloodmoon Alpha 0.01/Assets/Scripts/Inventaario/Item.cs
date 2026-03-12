using UnityEngine;

public enum SlotTag
{
    None,
    Head,
    Chest,
    Legs,
    Feet,
    Stackable,
    Food,
    Water
}

[CreateAssetMenu(menuName = "Scriptable Object/Item")]
public class Item : ScriptableObject
{
    [Header("Basic Info")]
    public Sprite sprite;
    public SlotTag itemTag;

    [Header("Consumable Settings")]
    [Tooltip("How much food this item restores when used.")]
    public int foodRestore;
    [Tooltip("How much water this item restores when used.")]
    public int waterRestore;

    [Header("Equipment")]
    public GameObject equipmentPrefab;   // Spawned when equipped

    [Header("Tool Settings")]
    public ToolType toolType;

    [Header("Weapon Settings")]
    public WeaponType weaponType;

    [Tooltip("What ammo this weapon uses (for bows)")]
    public Item ammoType;

    [Tooltip("Projectile prefab used by ammo (for arrows)")]
    public GameObject projectilePrefab;

    public bool IsStackableItem()
    {
        return itemTag == SlotTag.Stackable ||
               itemTag == SlotTag.Food ||
               itemTag == SlotTag.Water;
    }

    public int GetMaxStackSize()
    {
        if (itemTag == SlotTag.Food || itemTag == SlotTag.Water)
            return 3;

        if (itemTag == SlotTag.Stackable)
            return 64;

        return 1;
    }
}
