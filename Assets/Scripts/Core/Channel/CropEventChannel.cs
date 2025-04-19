using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/CropEventChannel")]
public class CropEventChannel : ScriptableObject
{
    public event UnityAction<int, TileModel> OnPlantSeed;

    public void RaisePlantSeed(int itemId, TileModel tileModel)
    {
        OnPlantSeed?.Invoke(itemId, tileModel);
    }
}
