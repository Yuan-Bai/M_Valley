using System;
using System.Collections.Generic;

public class InventoryModel
{
    // itemID -> quantity
    private Dictionary<int, int> items = new();

    public Action<Dictionary<int, int>> OnInventoryUpdated;

    public void AddItem(int itemID, int quantity)
    {
        if (items.ContainsKey(itemID))
        {
            items[itemID] += quantity;
            OnInventoryUpdated?.Invoke(items);
        }
    }

    public bool TryAddItem(int itemID, int quantity)
    {
        items.Add(itemID, quantity);
        OnInventoryUpdated?.Invoke(items);
        return true;
    }

    public void RemoveItem(int itemID, int quantity)
    {
        if (items.ContainsKey(itemID))
        {
            items[itemID] -= quantity;
            OnInventoryUpdated?.Invoke(items);
        }
    }
}
