using System;
using System.Collections.Generic;
using System.Linq;

public class ItemSlotModel
{
    private int _itemID;
    private int _count;
    public int SlotIndex {get;}

    public int ItemID
    {
        get => _itemID;
        set => _itemID = value >= -1 ? value : throw  new ArgumentException("Invalid itemID");
    }

    public int Count
    {
        get => _count;
        set => _count = value >= 0 ? value : throw new ArgumentException("itemCount cannot be negative");
    }

    public bool IsEmpty => _count == 0;

    public ItemSlotModel(int slotIndex)
    {
        SlotIndex = slotIndex;
        ItemSlotClear();
    }

    public void ItemSlotClear()
    {
        ItemID = -1;
        Count = 0;
    }
}

public struct SlotChangeData
{
    public int SlotIndex;
    public int PreviousItemId;
    public int PreviousCount;
    public int NewItemId;
    public int NewCount;
}

public class InventoryModel
{
    private readonly ItemDatabase_SO _itemDatabase;
    private readonly InventoryEventChannel _InventoryEventChannel;
    private readonly ItemSlotModel[] _slots;
    private readonly HashSet<int> _changedSlots = new();
    private readonly List<SlotChangeData> _slotChanges = new();


    public int Capacity => _slots.Length;
    public IReadOnlyList<ItemSlotModel> Slots => Array.AsReadOnly(_slots);

    public InventoryModel(ItemDatabase_SO database, InventoryEventChannel eventChannel, int capacity)
    {
        _itemDatabase = database != null ? database : throw new ArgumentNullException(nameof(database));
        _InventoryEventChannel = eventChannel != null ? eventChannel : throw new ArgumentNullException(nameof(eventChannel));
        
        _slots = new ItemSlotModel[capacity];
        for (int i = 0; i < capacity; i++)
        {
            _slots[i] = new(i);
        }
    }

    public int AddItem(int itemId, int quantity)
    {
        var itemData = _itemDatabase.GetItemByID(itemId);
        if (itemData == null) return quantity;

        int remaining = quantity;
        _slotChanges.Clear(); // 清空上次变更记录

        remaining = TrackExistingStackChanges(itemId, remaining, itemData.MaxStack);
        if (remaining == 0)
        {
            RaiseDeltaUpdate();
            return 0;
        }

        remaining = TrackNewSlotChanges(itemId, remaining, itemData.MaxStack);
        RaiseDeltaUpdate();
        return remaining;
    }

    public int RemoveItemFromSlot(int slotIndex, int quantity)
    {
        if (_slots[slotIndex].IsEmpty) return quantity;
        int originalCount = _slots[slotIndex].Count;
        int remaining = quantity > originalCount ? 0 : originalCount-quantity;
        _slots[slotIndex].Count = remaining;
        int id = _slots[slotIndex].ItemID;
        if (remaining == 0)
        {
            _slots[slotIndex].ItemSlotClear();
        }
        RecordSlotChange(_slots[slotIndex], originalCount, id);
        RaiseDeltaUpdate();
        return remaining;
    }

    public void SwapItem(int slotIndex1, int slotIndex2)
    {
        int id1 = _slots[slotIndex1].ItemID;
        int id2 = _slots[slotIndex2].ItemID;
        if (id1 == id2)
        {
            int id = id1;
            int originalCount1 = _slots[slotIndex1].Count;
            int originalCount2 = _slots[slotIndex2].Count;
            int canAdd = _itemDatabase.GetItemByID(id).MaxStack - originalCount2;
            int addAmount = Math.Min(canAdd, originalCount1);
            _slots[slotIndex2].Count += addAmount;
            if (originalCount1 < addAmount)
            {
                _slots[slotIndex1].ItemSlotClear();
            }
            else
            {
                _slots[slotIndex1].Count -= addAmount;
            }
            RecordSlotChange(_slots[slotIndex1], originalCount1, id);
            RecordSlotChange(_slots[slotIndex2], originalCount2, id);
            RaiseDeltaUpdate();
        }
        else
        {
            int count1 = _slots[slotIndex1].Count;
            int count2 = _slots[slotIndex2].Count;
            _slots[slotIndex1].ItemID = id2;
            _slots[slotIndex2].ItemID = id1;
            _slots[slotIndex1].Count = count2;
            _slots[slotIndex2].Count = count1;
            RecordSlotChange(_slots[slotIndex1], count1, id1);
            RecordSlotChange(_slots[slotIndex2], count2, id2);
            RaiseDeltaUpdate();
        }
    }

    private int TrackExistingStackChanges(int itemId, int remaining, int maxStack)
    {
        foreach (var slot in _slots.Where(s => s.ItemID == itemId))
        {
            if (remaining <= 0) break;

            int originalCount = slot.Count;
            int canAdd = maxStack - originalCount;
            if (canAdd <= 0) continue;

            int addAmount = Math.Min(remaining, canAdd);
            slot.Count += addAmount;
            remaining -= addAmount;

            RecordSlotChange(slot, originalCount);
        }
        return remaining;
    }

    private int TrackNewSlotChanges(int itemId, int remaining, int maxStack)
    {
        foreach (var slot in _slots.Where(s => s.IsEmpty))
        {
            if (remaining <= 0) break;

            int addAmount = Math.Min(remaining, maxStack);
            var originalId = slot.ItemID;
            var originalCount = slot.Count;

            slot.ItemID = itemId;
            slot.Count = addAmount;
            remaining -= addAmount;

            RecordSlotChange(slot, originalCount, originalId);
        }
        return remaining;
    }

    private void RecordSlotChange(ItemSlotModel slot, int originalCount, int? originalId = null)
    {
        var change = new SlotChangeData
        {
            SlotIndex = slot.SlotIndex,
            PreviousItemId = originalId ?? slot.ItemID,
            PreviousCount = originalCount,
            NewItemId = slot.ItemID,
            NewCount = slot.Count
        };
        _slotChanges.Add(change);
        _changedSlots.Add(slot.SlotIndex);
    }

    private void RaiseDeltaUpdate()
    {
        if (_changedSlots.Count == 0) return;

        // 发送两种形式的数据：
        // 1. 完整变化的详细数据（用于日志/回放）
        // 2. 仅变化槽位的当前状态（用于UI更新）
        var changedSlots = _changedSlots.Select(i => _slots[i]).ToList();
        
        _InventoryEventChannel.RaiseInventoryDelta(
            deltaDetails: _slotChanges.AsReadOnly(),
            changedSlots: changedSlots.AsReadOnly()
        );

        _changedSlots.Clear();
        _slotChanges.Clear();
    }

    // 其他操作方法（移除、交换等）...
}