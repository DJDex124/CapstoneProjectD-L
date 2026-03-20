using UnityEngine;

public class ItemPrefab : MonoBehaviour
{

    public OldItemType itemType;
    public OldItemData itemData;

    private void Update()
    {
        if (PlayerMovementCC.current.canPickup && Input.GetKeyDown(KeyCode.E))
        {
            OldInventory.current.AddItem(itemData);
            Destroy(gameObject);
        }
    }

    
}

public enum OldItemType
{
    Stick,
    Stone,
    Leaf,
    // Add more item types as needed
}