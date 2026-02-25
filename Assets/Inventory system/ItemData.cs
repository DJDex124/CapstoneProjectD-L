using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    

    public GameObject pickupPrefab;
    

    public ItemType itemType;
    public enum ItemType
    {
        Consumable,
        Item,
        Tool,
        DontUse
    }
    

}
