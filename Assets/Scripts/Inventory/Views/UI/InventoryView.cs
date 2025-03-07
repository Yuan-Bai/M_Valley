using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private Transform itemSlotsParent;
    [SerializeField] private GameObject itemSlotPrefab;

    private Dictionary<int, ItemSlot> slots = new();

    public void Initialize(Dictionary<int, int> items)
    {
        // 初始化UI布局
    }

    public void UpdateView(Dictionary<int, int> updatedItems)
    {
        // 更新UI显示
    }
}

