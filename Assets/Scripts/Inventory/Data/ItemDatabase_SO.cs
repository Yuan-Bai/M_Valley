using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Database", menuName = "SO/Item Database")]
public class ItemDatabase_SO : ScriptableObject
{
    public List<ItemModel> itemDetailsList;

    public ItemModel GetItemByID(int itemID)
    {
        return itemDetailsList.Find(x => x.itemID == itemID);
    }

    public ItemModel GetItemByName(string itemName)
    {
        return itemDetailsList.Find(x => x.itemName == itemName);
    }
}
