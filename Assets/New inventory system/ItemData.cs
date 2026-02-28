using UnityEngine;

/// <summary>
/// ScriptableObject that defines an item's static data.
/// Create via: Right Click > Create > Inventory > Item Data
/// </summary>
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    public string itemName = "New Item";
    public string description = "";
    public Sprite icon;
    public GameObject worldPrefab; // prefab dropped into the world

    [Header("Stacking")]
    public bool isStackable = true;
    public int maxStackSize = 64;

    [Header("Classification")]
    public ItemType itemType = ItemType.Misc;
    public ItemRarity rarity = ItemRarity.Common;

    [Header("Weight (optional)")]
    public float weight = 0.1f;
}

public enum ItemType
{
    Weapon,
    Armour,
    Consumable,
    Tool,
    Resource,
    Misc
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

