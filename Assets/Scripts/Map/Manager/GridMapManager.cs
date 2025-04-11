using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GridMapManager : MonoBehaviour
{
    [Header("参数")]
    [SerializeField] private List<MapDataSO> mapDataSOList = new List<MapDataSO>();
    [SerializeField] private RuleTile digTile;
    [SerializeField] private RuleTile waterTile;

    [Header("事件通道")]
    [SerializeField] private SceneEventChannel _sceneEventChannel;
    [SerializeField] private GridEventChannel _gridEventChannel;

    #region 私有参数
    private Dictionary<string, TileModel> tileModelDict = new();
    private Tilemap _digTileMap;
    private Tilemap _waterTileMap;
    private Grid _currentGrid; 
    #endregion

    void OnEnable()
    {
        _sceneEventChannel.OnAfterSceneLoad += HandleAfterSceneLoad;
        _gridEventChannel.OnTileUpdate += HandleGridUpdate;
    }

    void OnDisable()
    {
        _sceneEventChannel.OnAfterSceneLoad -= HandleAfterSceneLoad;
        _gridEventChannel.OnTileUpdate -= HandleGridUpdate;
    }

    void Start()
    {
        foreach (var mapDataSO in mapDataSOList)
        {
            InitializeTileModelDict(mapDataSO);
        }
    }

    #region 事件
    private void HandleAfterSceneLoad()
    {
        _currentGrid = FindAnyObjectByType<Grid>();
        _digTileMap = GameObject.FindWithTag("Dig").GetComponent<Tilemap>();
        _waterTileMap = GameObject.FindWithTag("Water").GetComponent<Tilemap>();
    }

    private void HandleGridUpdate(ItemType itemType, Vector3 pos)
    {
        Vector3Int cellPos = _currentGrid.WorldToCell(pos);
        string key = SceneManager.GetActiveScene().name+'&'+cellPos.x+'X'+cellPos.y+'Y';
        tileModelDict.TryGetValue(key, out TileModel tileModel);
        switch (itemType)
        {
            case ItemType.HoeTool:
            if (tileModel.canDig)
                SetDigGround(tileModel);
            // 音效
            break;
            case ItemType.WaterTool:
            // if (tileModel.)
            SetWaterGround(tileModel);

            break;
        }
    }
    #endregion


    public void InitializeTileModelDict(MapDataSO mapDataSO)
    {
        foreach(TileProperty tileProperty in mapDataSO.tileProperties)
        {
            TileModel tileModel = new()
            {
                gridX = tileProperty.tileCoordinate.x,
                gridY = tileProperty.tileCoordinate.y,
            };

            switch (tileProperty.gridType)
            {
                case GridType.Diggable:
                    tileModel.canDig = tileProperty.boolTypeValue;
                    break;
                case GridType.Dropable:
                    tileModel.canDropItem = tileProperty.boolTypeValue;
                    break;
                case GridType.PlaceFurniture:
                    tileModel.canPlaceFurniture = tileProperty.boolTypeValue;
                    break;
                case GridType.NPCObstacle:
                    tileModel.isNPCObstacle = tileProperty.boolTypeValue;
                    break;
            }

            string key = mapDataSO.sceneName+'&'+tileModel.gridX+'X'+tileModel.gridY+'Y';
            if (!tileModelDict.TryGetValue(key, out TileModel tempTile))
            {
                tileModelDict.Add(key, tileModel);
                // todo
            }
        }
    }

    private void SetDigGround(TileModel tile)
    {
        Vector3Int pos = new(tile.gridX, tile.gridY, 0);
        if (_digTileMap != null)
        {
            _digTileMap.SetTile(pos, digTile);
            tile.daysSinceDug = 0;
        }
    }

    private void SetWaterGround(TileModel tile)
    {
        Vector3Int pos = new(tile.gridX, tile.gridY, 0);
        if (_waterTileMap != null)
        {
            _waterTileMap.SetTile(pos, waterTile);
        }
    }
}
