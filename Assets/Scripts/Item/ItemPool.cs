using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool : MonoSingleton<ItemPool>
{
    [SerializeField] private WorldItem itemPrefab;
    private Stack<WorldItem> pool = new();

    // public WorldItem GetItem(int itemID)
    // {
    //     var item = pool.Count > 0 ? pool.Pop() : Instantiate(itemPrefab);
    //     item.Initialize(itemID);
    //     item.gameObject.SetActive(true);
    //     return item;
    // }

    public void ReturnItem(WorldItem item)
    {
        item.gameObject.SetActive(false);
        pool.Push(item);
    }
}

