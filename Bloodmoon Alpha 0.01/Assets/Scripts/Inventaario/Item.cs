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

    [TextArea]
    public string description; // 🔥 NEW

    [Header("Consumable Settings")]
    public int foodRestore;
    public int waterRestore;

    [Header("Equipment")]
    public GameObject equipmentPrefab;

    [Header("Tool Settings")]
    public ToolType toolType;

    [Header("Weapon Settings")]
    public WeaponType weaponType;
    public Item ammoType;
    public GameObject projectilePrefab;

    [Header("Combat")]
    public float damage = 10f;
    public float knockbackForce = 1f;

    public bool IsStackableItem()
    {
        return itemTag == SlotTag.Stackable || itemTag == SlotTag.Food || itemTag == SlotTag.Water;
    }

    public int GetMaxStackSize()
    {
        if (itemTag == SlotTag.Food || itemTag == SlotTag.Water) return 3;
        if (itemTag == SlotTag.Stackable) return 64;
        return 1;
    }
}