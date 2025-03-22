using System;
using UnityEngine;
using UnityEngine.Events;


// ItemEventChannel.cs（使用ScriptableObject）
[CreateAssetMenu(fileName = "ItemEventChannel", menuName = "Events/ItemEventChannel")]
public class ItemEventChannel : ScriptableObject {
    public event UnityAction<ItemPickupData, Action<int>> OnItemPickedUp;
    public event UnityAction<int, int> OnItemDrop;
    public event UnityAction<int, int> OnItemSwap;
    public event UnityAction<ItemModel, bool> OnItemSelect;
    public event UnityAction<int, int, Vector2> OnCreateWorldItemWithVelocity;

    public void RaiseEvent(ItemPickupData data, Action<int> callback)
    {
        OnItemPickedUp?.Invoke(data, callback);
    }

    public void RaiseItemDrop(int slotIndex, int quantity)
    {
        OnItemDrop?.Invoke(slotIndex, quantity);
    }

    public void RaiseItemSwap(int slotIndex1, int slotIndex2)
    {
        OnItemSwap?.Invoke(slotIndex1, slotIndex2);
    }

    public void RasieItemSelect(ItemModel itemdata, bool isSelect)
    {
        OnItemSelect?.Invoke(itemdata, isSelect);
    }

    public void RaiseCreateWorldItemWithVelocity(int itemID, int quantity, Vector2 targetPos)
    {
        OnCreateWorldItemWithVelocity?.Invoke(itemID, quantity, targetPos);
    }
}

// 事件数据结构
public struct ItemPickupData {
    public int ItemID;
    public int Quantity;
}

