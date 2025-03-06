using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataList_SO", menuName = "Inventory/ItemDataList_SO")]
public class ItemDataList_SO : ScriptableObject
{
    public List<ItemDetails> itemDetailsList;

    public ItemDetails GetItemDetailsByID(int itemID)
    {
        return itemDetailsList.Find(x => x.itemID == itemID);
    }

    public ItemDetails GetItemDetailsByName(string itemName)
    {
        return itemDetailsList.Find(x => x.itemName == itemName);
    }
}
