using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/InventroyChannel")]
public class InventoryEventChannel : ScriptableObject
{
    // 使用自定义结构体避免外部修改
    public event UnityAction<IReadOnlyList<SlotChangeData>, IReadOnlyList<ItemSlotModel>> OnDeltaUpdate;
    public event UnityAction<IReadOnlyList<ItemSlotModel>> OnFullUpdate;

    public event UnityAction<ItemModel, bool> OnTipUpdate;

    public void RaiseInventoryUpdate(IReadOnlyList<ItemSlotModel> slots)
    {
        OnFullUpdate?.Invoke(slots);
    }

    public void RaiseInventoryDelta(
        IReadOnlyList<SlotChangeData> deltaDetails,
        IReadOnlyList<ItemSlotModel> changedSlots)
    {
        OnDeltaUpdate?.Invoke(deltaDetails, changedSlots);
    }

    public void RaiseTipUpdate(ItemModel itemData, bool isHide)
    {
        OnTipUpdate?.Invoke(itemData, isHide);
    }

    // 定义不可变数据结构
    public readonly struct InventoryUpdateData
    {
        public readonly IReadOnlyList<ItemSlotReadOnly> Slots;

        public InventoryUpdateData(IEnumerable<ItemSlotModel> slots)
        {
            Slots = slots.Select(s => new ItemSlotReadOnly(s)).ToArray();
        }
    }

    // 只读视图
    public class ItemSlotReadOnly
    {
        public readonly int ItemID;
        public readonly int Count;
        public readonly int SlotIndex;

        public ItemSlotReadOnly(ItemSlotModel slot)
        {
            ItemID = slot.ItemID;
            Count = slot.Count;
            SlotIndex = slot.SlotIndex;
        }
    }
}
