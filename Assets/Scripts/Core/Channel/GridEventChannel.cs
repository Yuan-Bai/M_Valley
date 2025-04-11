using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/GridEventChannel")]
public class GridEventChannel : ScriptableObject
{
    public event UnityAction<ItemType, Vector3> OnTileUpdate;

    public void RaiseTileUpdate(ItemType itemType, Vector3 pos)
    {
        OnTileUpdate?.Invoke(itemType, pos);
    }
}
