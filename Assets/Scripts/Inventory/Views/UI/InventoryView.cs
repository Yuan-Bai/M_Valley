using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Proxies;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class InventoryView : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private List<Transform> _itemSlotsParent;
    [SerializeField] private GameObject PlayerBag;

    private InventoryController _controller;
    private InventoryModel _model;
    private List<ItemSlot> _slots = new();
    private List<bool> isEmptyList = new();
    private Dictionary<int, HashSet<int>> _itemToSlots = new();
    private Dictionary<int, int> _slotToItem = new();

    
    IEnumerator Start() {
        while(CrossSceneService.InventoryController == null) {
            yield return null;
        }
        Initialize(CrossSceneService.InventoryController, CrossSceneService.InventoryController.Model);
    }

    public void Initialize(InventoryController controller, InventoryModel model)
    {
        _controller = controller;
        _model = model;
        _controller.RegisterView(this);
        InitializeSlots();
        _model.OnInventoryUpdated += OnInventoryUpdated;
    }

    public void InitializeSlots()
    {
        int slotIndex = 0;
        foreach (Transform slotParent in _itemSlotsParent)
        {
            foreach (Transform child in slotParent)
            {
                if (child.TryGetComponent<ItemSlot>(out ItemSlot slot))
                {
                    slot.Initialize(slotIndex, OnSlotClicked);
                    _slots.Add(slot);
                    isEmptyList.Add(true);
                    slotIndex++;
                }
            }
        }
    }

    private void OnSlotClicked(int slotIndex)
    {
        _controller.OnSlotClicked(slotIndex);
    }

    private void OnInventoryUpdated(Dictionary<int, ItemStackInfo> inventory)
    {
        ClearSlots();
        _slotToItem.Clear();
        _itemToSlots.Clear();
        foreach (var pair in inventory) {
            UpdateItemSlots(pair.Key, pair.Value);
        }
        // // 增量更新逻辑
        // HashSet<int> processedItems = new();
        
        // foreach (var pair in inventory) {
        //     UpdateItemSlots(pair.Key, pair.Value);
        //     processedItems.Add(pair.Key);
        // }

        // // 清理已不存在的物品
        // foreach (var itemID in _itemToSlots.Keys.Except(processedItems).ToList())
        // {
        //     RemoveItemFromView(itemID);
        // }
    }

    private void ClearSlots()
    {
        foreach(ItemSlot slot in _slots)
        {
            slot.Clear();
        }
    }

    private void UpdateItemSlots(int itemID, ItemStackInfo stackInfo)
    {
        _itemToSlots.Add(itemID, new HashSet<int>());
        foreach (var slotStack in stackInfo.Slots)
        {
            _slots[slotStack.SlotIndex].UpdateItem(_controller.GetItemData(itemID), slotStack.StackCount);
            _slotToItem[slotStack.SlotIndex] = itemID;
            _itemToSlots[itemID].Add(slotStack.SlotIndex);
        }

        // // 1. 同步现有slot
        // int slotIndex = 0;
        // foreach (var slotStack in stackInfo.Slots) {
        //     if (!_itemToSlots.ContainsKey(itemID))
        //     {
        //         slotStack.SlotIndex = AddNewSlotForItem(itemID, slotStack.StackCount);
        //     }
        //     else if (slotIndex >= _itemToSlots[itemID].Count)
        //     {
        //         // 需要新slot
        //         slotStack.SlotIndex = AddNewSlotForItem(itemID, slotStack.StackCount);
        //     }
        //     else
        //     {
        //         // 更新现有slot
        //         int existingSlot = _itemToSlots[itemID].ElementAt(slotIndex);
        //         _slots[existingSlot].UpdateItem(_controller.GetItemData(itemID), slotStack.StackCount);
        //     }
        //     slotIndex++;
        // }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemID"></param>
    /// <param name="count"></param>
    /// <returns>返回添加的slot的索引</returns>
    private int AddNewSlotForItem(int itemID, int count) {
        int slotIndex = FindAvailableSlotIndex();
        if (slotIndex == -1) {
            Debug.LogError("Inventory full!");
            return -1;
        }

        _slots[slotIndex].UpdateItem(_controller.GetItemData(itemID), count);
        _slotToItem[slotIndex] = itemID;
        if (!_itemToSlots.ContainsKey(itemID))
        {
            _itemToSlots.Add(itemID, new HashSet<int>());
        }
        _itemToSlots[itemID].Add(slotIndex);
        isEmptyList[slotIndex] = false;
        return slotIndex;
    }

    private void RemoveItemFromView(int itemID)
    {
        foreach (int slotIndex in _itemToSlots[itemID])
        {
            _slots[slotIndex].Clear();
            _slotToItem.Remove(slotIndex);
            isEmptyList[slotIndex] = true;
        }
        _itemToSlots.Remove(itemID);
    }

    public bool TryGetItemInSlot(int slotIndex, out int itemID) {
        return _slotToItem.TryGetValue(slotIndex, out itemID);
    }

    public void SetSlotHighlight(int slotIndex, bool state) {
        _slots[slotIndex].SetHighlight(state);
    }

    private int FindAvailableSlotIndex()
    {
        for (int i = 0; i < isEmptyList.Count; i++)
        {
            if (isEmptyList[i])
                return i;
        }
        return -1;
    }

    public void ShowOrHideBag()
    {
        PlayerBag.SetActive(!PlayerBag.activeSelf);
    }
}

