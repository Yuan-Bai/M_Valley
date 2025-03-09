using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    [Header("UI参数")]
    [SerializeField] private Transform itemSlotsParent;

    private List<ItemSlot> slots = new();  // 改用List存储按顺序排列的格子
    private bool[] isEmptyArray; 
    private int maxSlotCount;
    private Dictionary<int, int> itemSlotMap = new();  // 物品ID到格子索引的映射

    void Awake()
    {
        InitializeSlots();
    }

    public void InitializeSlots()
    {

        foreach (Transform child in itemSlotsParent)
        {
            if (child.TryGetComponent<ItemSlot>(out ItemSlot slot))
            {
                slots.Add(slot);
            }
        }
        maxSlotCount = slots.Count;
        isEmptyArray = new bool[maxSlotCount];
        for (int i = 0; i < maxSlotCount; i++)
        {
            isEmptyArray[i] = true;
        }
    }

    // public void InitializeAsync(Dictionary<int, int> items)
    // {
    //     // 初始化UI布局

    //     // 是否需要重置
        
    //     //
    //     int slotIndex = 0;
    //     foreach (var item in items)
    //     {
    //         if (slotIndex > maxSlotCount) break;
    //         InventoryController.Instance.GetItemModelAsync(item.Key, model =>
    //         {
    //             slots[slotIndex].Initialize(model, item.Value);
    //         });
    //     }
    // }

    public void Initialize(Dictionary<int, int> items)
    {
        ResetAllSlots();
        int slotIndex = 0;
        foreach (var item in items)
        {
            if (slotIndex > maxSlotCount) break;
            slots[slotIndex].Initialize(InventoryController.Instance.GetItemModel(item.Key), item.Value);
            itemSlotMap[item.Key] = slotIndex;
            isEmptyArray[slotIndex] = false;
            slotIndex++;
        }
    }

    public void UpdateView(Dictionary<int, int> updatedItems)
    {
        foreach(var pair in itemSlotMap.ToList())
        {
            int itemID = pair.Key;
            int slotIndex = pair.Value;

            if (updatedItems.TryGetValue(itemID, out int newCount))
            {
                if (newCount > 0)
                {
                    slots[slotIndex].UpdateCount(newCount);
                }
                else
                {
                    slots[slotIndex].gameObject.SetActive(false);
                    itemSlotMap.Remove(itemID);
                }
            }
            else
            {
                // 物品不存在时释放格子
                slots[slotIndex].gameObject.SetActive(false);
                itemSlotMap.Remove(itemID);
                isEmptyArray[slotIndex] = true;
            }
        }

        // 处理新增物品
        foreach (var item in updatedItems)
        {
            if (!itemSlotMap.ContainsKey(item.Key))
            {
                int availableIndex = FindAvailableSlotIndex();
                if (availableIndex == -1)
                {
                    Debug.LogWarning("背包已满，无法添加新物品！");
                    break;
                }

                slots[availableIndex].gameObject.SetActive(true);
                slots[availableIndex].Initialize(
                    InventoryController.Instance.GetItemModel(item.Key),
                    item.Value
                );
                itemSlotMap.Add(item.Key, availableIndex);
                isEmptyArray[availableIndex] = false;
            }
        }
    }

    // 查找第一个可用空格子
    private int FindAvailableSlotIndex()
    {
        for (int i = 0; i < maxSlotCount; i++)
        {
            if (isEmptyArray[i])
                return i;
        }
        return -1;
    }

    // 重置所有格子状态
    private void ResetAllSlots()
    {
        foreach (ItemSlot slot in slots)
        {
            slot.gameObject.SetActive(false);
        }
        itemSlotMap.Clear();
        for (int i = 0; i < maxSlotCount; i++)
        {
            isEmptyArray[i] = true;
        }
    }
}

