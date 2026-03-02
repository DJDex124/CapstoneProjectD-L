using UnityEngine;

public class ClothingSlot : MonoBehaviour
{
    public ItemData equippedItem;
    public ItemContainer container;

    public void Equip(ItemData item)
    {
        equippedItem = item; 
        if (item.hasStorage)
            container = new ItemContainer(item.storageRows, item.storageColumns);
        else
            container = null;
    }

    public void Unequip()
    {
        equippedItem = null;
        container = null;
    }   
    public bool isEmpty => equippedItem == null;
}
