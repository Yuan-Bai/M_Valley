using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/GridEventChannel")]
public class GridEventChannel : ScriptableObject
{
    public event UnityAction<ItemModel, Vector3> OnTileUpdate;

    public void RaiseTileUpdate(ItemModel itemModel, Vector3 pos)
    {
        OnTileUpdate?.Invoke(itemModel, pos);
    }
}
