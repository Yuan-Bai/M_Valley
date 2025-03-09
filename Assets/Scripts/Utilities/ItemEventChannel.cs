using System;
using UnityEngine;
using UnityEngine.Events;


// ItemEventChannel.cs（使用ScriptableObject）
[CreateAssetMenu(fileName = "ItemEventChannel", menuName = "Events/Item Event Channel")]
public class ItemEventChannel : ScriptableObject {
    public UnityAction<ItemPickupData, Action<int>> OnItemPickedUp;

    public void RaiseEvent(ItemPickupData data, Action<int> callback) {
        OnItemPickedUp?.Invoke(data, callback);
    }
}

// 事件数据结构
public struct ItemPickupData {
    public int ItemID;
    public int Quantity;
}

