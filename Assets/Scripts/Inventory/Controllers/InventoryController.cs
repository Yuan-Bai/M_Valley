using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class InventoryController : MonoSingleton<InventoryController>
{
    [SerializeField] private ItemDatabase_SO itemDatabase;
    private InventoryModel model;
    private InventoryView view;
    private Dictionary<int, ItemModel> cache = new();

    protected override void Awake()
    {
        base.Awake();
        model = new InventoryModel();
        view = GetComponentInChildren<InventoryView>();
        
        // 从服务加载数据
        // var savedData = InventorySaveService.Instance.Load();
        // model.Initialize(savedData);
    }

    private void HandleInventoryUpdate(Dictionary<int, int> items)
    {
        view.UpdateView(items);
    }

    // 对外暴露的交互接口
    public void AddItemToInventory(int itemID, int quantity)
    {
        model.AddItem(itemID, quantity);
    }

    public bool TryAddItemToInventory(int itemID, int quantity)
    {
        return model.TryAddItem(itemID, quantity);
    }
    
    // 其他交互方法...
    public ItemModel GetItemModel(int itemID)
    {
        return itemDatabase.GetItemByID(itemID);
    }

    public void GetItemModelAsync(int itemID, Action<ItemModel> callback)
    {
        if (itemID < 0)
        {
            callback?.Invoke(null);
            return;
        }

        if (cache.TryGetValue(itemID, out var cachedModel))
        {
            callback?.Invoke(cachedModel);
            return;
        }

        StartCoroutine(FetchItemModelRoutine(itemID, callback));
    }

    private IEnumerator FetchItemModelRoutine(int itemID, Action<ItemModel> callback)
    {
        bool isCompleted = false;
        ItemModel result = null;
        Exception error = null;

        Task.Run(() =>
        {
            try
            {
                result = itemDatabase.GetItemByID(itemID);
            }
            catch (Exception e)
            {
                error = e;
            }
            finally
            {
                isCompleted = true;
            }
        });

        float timeout = 5f;
        float timer = 0f;

        while (!isCompleted && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (error != null)
        {
            Debug.LogError($"查询失败: {error.Message}");
            callback?.Invoke(null);
            yield break;
        }

        if (result != null)
        {
            cache[itemID] = result;
        }

        MainThreadDispatcher.Enqueue(() => callback?.Invoke(result));
    }
}

