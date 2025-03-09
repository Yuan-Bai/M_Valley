using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryModel
{
    public event Action<Dictionary<int, ItemStackInfo>> OnInventoryUpdated;

    private Dictionary<int, ItemStackInfo> _items = new();
    private readonly ItemDatabase_SO _itemDatabase;
    private int _maxSize = 40;

    public InventoryModel(ItemDatabase_SO database)
    {
        _itemDatabase = database;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemID"></param>
    /// <param name="quantity"></param>
    /// <returns>未添加的数量</returns>
    public int TryAddItem(int itemID, int quantity)
    {
        if (CheckFull())
        {
            return quantity;
        }
        var itemData = _itemDatabase.GetItemByID(itemID);
        if (itemData == null) return quantity;
        if (!_items.TryGetValue(itemID, out ItemStackInfo stackInfo))
        {
            stackInfo = new ItemStackInfo();
            _items[itemID] = stackInfo;
        }

        int remaining = quantity;
        foreach (var slot in stackInfo.Slots)
        {
            int canAdd = itemData.MaxStack - slot.StackCount;
            if (canAdd <= 0) continue;

            int addAmount = Mathf.Min(remaining, canAdd);
            slot.StackCount += addAmount;
            stackInfo.TotalCount += addAmount;
            remaining -= addAmount;
            if (remaining == 0) break;
        }
        while (remaining > 0 && !CheckFull())
        {
            int newStack = Mathf.Min(remaining, itemData.MaxStack);
            stackInfo.Slots.Add(new ItemStackInfo.SlotStack{
                SlotIndex = -1,
                StackCount = newStack
            });
            stackInfo.TotalCount += newStack;
            remaining -= newStack;
        }
        OnInventoryUpdated?.Invoke(_items);
        return remaining;
    }

    public void RemoveItem(int itemID, int quantity)
    {
        if (_items.TryGetValue(itemID, out ItemStackInfo stackInfo)) return;

        int remaining = quantity;
        for (int i = stackInfo.Slots.Count-1; i >= 0; i--)
        {
            var slot = stackInfo.Slots[i];
            int removeAmount = Mathf.Min(remaining, slot.StackCount);
            
            slot.StackCount -= removeAmount;
            stackInfo.TotalCount -= removeAmount;
            remaining -= removeAmount;

            if (slot.StackCount == 0)
            {
                stackInfo.Slots.RemoveAt(i);
            }
            if (remaining == 0) break;
        }

        if (stackInfo.Slots.Count == 0)
        {
            _items.Remove(itemID);
        }
        OnInventoryUpdated?.Invoke(_items);
    }

    private bool CheckFull()
    {
        int slotUsedCount = 0;
        foreach (var pair in _items)
        {
            slotUsedCount += pair.Value.Slots.Count;
        }
        return slotUsedCount == _maxSize;
    }
}
