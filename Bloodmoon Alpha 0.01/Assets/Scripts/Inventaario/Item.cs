using UnityEngine;

public enum SlotTag
{
    None,
    Head,
    Chest,
    Legs,
    Feet,
    Stackable
}

[CreateAssetMenu(menuName = "Scriptable Object/Item")]
public class Item : ScriptableObject
{
    [Header("Basic Info")]
    public Sprite sprite;
    public SlotTag itemTag;

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
}