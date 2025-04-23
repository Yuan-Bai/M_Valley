using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDataSO", menuName = "SO/MapDataSO")]
public class MapDataSO : ScriptableObject
{
    [SceneName] public string sceneName;
    public int gridWidth;
    public int gridHeight;

    [Header("左下角点的位置")]
    public Vector2Int originalPos;

    public List<TileProperty> tileProperties;
}
