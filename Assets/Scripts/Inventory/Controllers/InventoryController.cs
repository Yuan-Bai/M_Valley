using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class InventoryController : MonoBehaviour
{
    [SerializeField] private ItemDatabase_SO _itemDatabase;
    [SerializeField] private InventoryView _view;
    [Header("事件")]
    [SerializeField] private ItemEventChannel _itemEventChannel;
    [SerializeField] private InventoryEventChannel _InventoryEventChannel;
    [SerializeField] private PlayerEventChannel _playerEventChannel;
    
    private InventoryModel _model;
    private HashSet<int> _selectedSlots = new();
    private Dictionary<int, ItemModel> _cache = new();
    public InventoryModel Model => _model;

    void Awake()
    {
        _model = new InventoryModel(_itemDatabase, _InventoryEventChannel, Settings.InventoryCapcity);
        CrossSceneService.InventoryController = this;
    }

    void OnEnable()
    {
        StartCoroutine(DelayedSubscribe());

        _itemEventChannel.OnItemPickedUp += HandleItemPickup;
        _itemEventChannel.OnItemDrop += HandleItemDrop;
        _itemEventChannel.OnItemSwap += HandleItemSwap;
    }

    void OnDisable()
    {
        if (InputManager.IsAvailable)
        {
            InputManager.Instance.Controls.UI.Inventory.performed -= OnInventoryPerformed;
            InputManager.Instance.Controls.UI.Disable();
        }

        _itemEventChannel.OnItemPickedUp -= HandleItemPickup;
        _itemEventChannel.OnItemDrop -= HandleItemDrop;
        _itemEventChannel.OnItemSwap -= HandleItemSwap;
    }

    private IEnumerator DelayedSubscribe()
    {
        yield return new WaitUntil(() => InputManager.IsAvailable);
        InputManager.Instance.Controls.UI.Enable();
        InputManager.Instance.Controls.UI.Inventory.performed += OnInventoryPerformed;
    }

    public void RegisterView(InventoryView view)
    {
        _view = view;
    }

    /// <summary>
    /// 添加物品并返回未添加成功的物品数量
    /// </summary>
    /// <param name="data"></param>
    /// <param name="callback"></param>
    private void HandleItemPickup(ItemPickupData data, Action<int> callback) {
        callback?.Invoke(_model.AddItem(data.ItemID, data.Quantity));
    }

    private void HandleItemDrop(int slotIndedx, int quantity)
    {
        _model.RemoveItemFromSlot(slotIndedx, quantity);
    }

    private void HandleItemSwap(int slotIndex1, int slotIndex2)
    {
        _model.SwapItem(slotIndex1, slotIndex2);
    }

    public void OnSlotClicked(int slotIndex)
    {
        if (_view.TryGetItemInSlot(slotIndex, out int itemID)) {
            ToggleSelection(slotIndex, itemID);
        }
    }

    private void OnInventoryPerformed(InputAction.CallbackContext context)
    {
        _view.ShowOrHideBag();
    }

    private void ToggleSelection(int slotIndex, int itemID)
    {
        if (_selectedSlots.Contains(slotIndex))
        {
            ClearAllSelections();
            HoldItem(itemID, false);
        }
        else
        {
            ClearAllSelections();
            _selectedSlots.Add(slotIndex);
            _view.SetSlotHighlight(slotIndex, true);

            HoldItem(itemID, true);
        }
    }

    private void HoldItem(int itemID, bool isSelect)
    {
        _itemEventChannel.RasieItemSelect(_itemDatabase.GetItemByID(itemID), isSelect);
        _playerEventChannel.RaiseHoldItem(_itemDatabase.GetItemByID(itemID), isSelect);
    }

    private void ClearAllSelections() {
        foreach (var slot in _selectedSlots) {
            _view.SetSlotHighlight(slot, false);
        }
        _selectedSlots.Clear();
    }

    // 外部接口示例
    public void AddItems(int itemID, int count) => _model.AddItem(itemID, count);
    public void RemoveSelectedItems() {
        foreach (var slotIndex in _selectedSlots) {
            if (_view.TryGetItemInSlot(slotIndex, out int itemID)) {
                // _model.RemoveItem(itemID, 1); // 按当前实现每次移除一个
            }
        }
        ClearAllSelections();
    }

    public ItemModel GetItemData(int itemID) {
        return _itemDatabase.GetItemByID(itemID);
    }
}

































// public class InventoryController : MonoSingleton<InventoryController>
// {
//     [SerializeField] private ItemDatabase_SO itemDatabase;
//     private InventoryModel model;
//     private InventoryView view;
//     private Dictionary<int, ItemModel> cache = new();

//     protected override void Awake()
//     {
//         base.Awake();
//         model = new InventoryModel();
//         view = GetComponentInChildren<InventoryView>();
        
//         // 从服务加载数据
//         // var savedData = InventorySaveService.Instance.Load();
//         // model.Initialize(savedData);
//     }

//     // 对外暴露的交互接口
//     public void AddItemToInventory(int itemID, int quantity)
//     {
//         model.AddItem(itemID, quantity);
//     }

//     public bool TryAddItemToInventory(int itemID, int quantity)
//     {
//         return model.TryAddItem(itemID, quantity);
//     }
    
//     // 其他交互方法...
//     public ItemModel GetItemModel(int itemID)
//     {
//         return itemDatabase.GetItemByID(itemID);
//     }

//     public void GetItemModelAsync(int itemID, Action<ItemModel> callback)
//     {
//         if (itemID < 0)
//         {
//             callback?.Invoke(null);
//             return;
//         }

//         if (cache.TryGetValue(itemID, out var cachedModel))
//         {
//             callback?.Invoke(cachedModel);
//             return;
//         }

//         StartCoroutine(FetchItemModelRoutine(itemID, callback));
//     }

//     private IEnumerator FetchItemModelRoutine(int itemID, Action<ItemModel> callback)
//     {
//         bool isCompleted = false;
//         ItemModel result = null;
//         Exception error = null;

//         Task.Run(() =>
//         {
//             try
//             {
//                 result = itemDatabase.GetItemByID(itemID);
//             }
//             catch (Exception e)
//             {
//                 error = e;
//             }
//             finally
//             {
//                 isCompleted = true;
//             }
//         });

//         float timeout = 5f;
//         float timer = 0f;

//         while (!isCompleted && timer < timeout)
//         {
//             timer += Time.deltaTime;
//             yield return null;
//         }

//         if (error != null)
//         {
//             Debug.LogError($"查询失败: {error.Message}");
//             callback?.Invoke(null);
//             yield break;
//         }

//         if (result != null)
//         {
//             cache[itemID] = result;
//         }

//         MainThreadDispatcher.Enqueue(() => callback?.Invoke(result));
//     }
// }

