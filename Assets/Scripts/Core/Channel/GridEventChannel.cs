using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/GridEventChannel")]
public class GridEventChannel : ScriptableObject
{
    public event UnityAction<ItemModel, Vector3> OnTileUpdate;

    public event UnityAction<System.Action<int, int, int, int>> OnGridDimensionsRequest;

    public event UnityAction<System.Action<Dictionary<string, TileModel>>> OnTileModelDictRequest;

    public void RaiseTileUpdate(ItemModel itemModel, Vector3 pos)
    {
        OnTileUpdate?.Invoke(itemModel, pos);
    }

    public void RaiseGridDimensionsRequest(System.Action<int, int, int, int> callBack)
    {
        OnGridDimensionsRequest?.Invoke(callBack);
    }

    public void RaiseTileModelDictRequest(System.Action<Dictionary<string, TileModel>> callBack)
    {
        OnTileModelDictRequest?.Invoke(callBack);
    }
}
