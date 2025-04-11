using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDataSO", menuName = "SO/MapDataSO")]
public class MapDataSO : ScriptableObject
{
    [SceneName] public string sceneName;

    public List<TileProperty> tileProperties;
}
