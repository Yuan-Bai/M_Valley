using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private List<Transform> _itemSlotsParent;
    [SerializeField] private GameObject PlayerBag;
    [SerializeField] private ItemToolTip itemToolTip;

    [Header("事件")]
    [SerializeField] private InventoryEventChannel _InventoryEventChannel;

    private InventoryController _controller;
    private List<ItemSlot> _slots = new();
    private Dictionary<int, HashSet<int>> _itemToSlots = new();
    private Dictionary<int, int> _slotToItem = new();

    void OnEnable()
    {
        _InventoryEventChannel.OnDeltaUpdate += HandleDeltaUpdate;
        _InventoryEventChannel.OnFullUpdate += HandleFullUpdate;
        _InventoryEventChannel.OnTipUpdate += HandleTipUpdate;
    }

    void OnDisable()
    {
        _InventoryEventChannel.OnDeltaUpdate -= HandleDeltaUpdate;
        _InventoryEventChannel.OnFullUpdate -= HandleFullUpdate;
        _InventoryEventChannel.OnTipUpdate += HandleTipUpdate;
    }

    IEnumerator Start() {
        yield return new WaitUntil(() => CrossSceneService.InventoryController != null);
        Initialize(CrossSceneService.InventoryController);
    }

    public void Initialize(InventoryController controller)
    {
        _controller = controller;
        _controller.RegisterView(this);
        InitializeSlots();
    }

    private void InitializeSlots()
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
                    slotIndex++;
                }
            }
        }
    }

    private void OnSlotClicked(int slotIndex)
    {
        _controller.OnSlotClicked(slotIndex);
    }

    private void HandleDeltaUpdate(
        IReadOnlyList<SlotChangeData> details, 
        IReadOnlyList<ItemSlotModel> changedSlots)
    {
        foreach (var slotModel in changedSlots)
        {
            UpdateSingleSlot(slotModel);
        }
    }

    private void HandleFullUpdate(IReadOnlyList<ItemSlotModel> slots)
    {
        foreach (var slotModel in slots)
        {
            UpdateSingleSlot(slotModel);
        }
    }

    private void HandleTipUpdate(ItemModel itemData, bool isHide)
    {
        if (isHide)
        {
            itemToolTip.gameObject.SetActive(false);
        }
        else
        {
            itemToolTip.UpdateTip(itemData);
            itemToolTip.gameObject.SetActive(true);
        }
    }

    private void UpdateSingleSlot(ItemSlotModel slotModel)
    {
        int slotIndex = slotModel.SlotIndex;
        int itemId = slotModel.ItemID;
        int count = slotModel.Count;

        // 更新视图元素
        ItemSlot slotUI = _slots[slotIndex];
        bool isEmpty = itemId == -1;

        // 更新字典跟踪
        if (_slotToItem.TryGetValue(slotIndex, out int prevItemId))
        {
            // 移除旧物品的槽位引用
            if (_itemToSlots.TryGetValue(prevItemId, out var slots))
            {
                slots.Remove(slotIndex);
                if (slots.Count == 0) _itemToSlots.Remove(prevItemId);
            }
            _slotToItem.Remove(slotIndex);
        }

        if (!isEmpty)
        {
            // 更新新物品的槽位引用
            if (!_itemToSlots.TryGetValue(itemId, out var slots))
            {
                slots = new HashSet<int>();
                _itemToSlots[itemId] = slots;
            }
            slots.Add(slotIndex);
            _slotToItem[slotIndex] = itemId;

            // 更新UI显示
            slotUI.UpdateItem(_controller.GetItemData(itemId), count);
        }
        else
        {
            slotUI.Clear();
        }
    }

    public bool TryGetItemInSlot(int slotIndex, out int itemID) 
    {
        return _slotToItem.TryGetValue(slotIndex, out itemID);
    }

    public void SetSlotHighlight(int slotIndex, bool state) 
    {
        _slots[slotIndex].SetHighlight(state);
    }

    public void ShowOrHideBag()
    {
        PlayerBag.SetActive(!PlayerBag.activeSelf);
    }

    // 调试方法
    public void PrintInventoryState()
    {
        Debug.Log($"Tracked Items: {_itemToSlots.Count}");
        foreach (var kvp in _itemToSlots)
        {
            Debug.Log($"Item {kvp.Key} in slots: {string.Join(",", kvp.Value)}");
        }
    }
}
