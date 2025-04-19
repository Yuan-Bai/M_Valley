using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.XR;

public class GridMapManager : MonoBehaviour
{
    [Header("参数")]
    [SerializeField] private List<MapDataSO> mapDataSOList = new List<MapDataSO>();
    [SerializeField] private RuleTile digTile;
    [SerializeField] private RuleTile waterTile;

    [Header("事件通道")]
    [SerializeField] private SceneEventChannel _sceneEventChannel;
    [SerializeField] private GridEventChannel _gridEventChannel;
    [SerializeField] private TimeEventChannel _timeEventChannel;
    [SerializeField] private CropEventChannel _cropEventChannel;
    [SerializeField] private ItemEventChannel _itemEventChannel;

    #region 私有参数
    private Dictionary<string, TileModel> tileModelDict = new();
    private Tilemap _digTileMap;
    private Tilemap _waterTileMap;
    private Grid _currentGrid; 
    private Season _currentSeason;
    #endregion

    void OnEnable()
    {
        _sceneEventChannel.OnAfterSceneLoad += HandleAfterSceneLoad;
        _gridEventChannel.OnTileUpdate += HandleGridUpdate;
        _timeEventChannel.OnDayChanged += HandleDayChanged;
        _timeEventChannel.OnSeasonChanged += HandleSeasonChanged;
    }

    void OnDisable()
    {
        _sceneEventChannel.OnAfterSceneLoad -= HandleAfterSceneLoad;
        _gridEventChannel.OnTileUpdate -= HandleGridUpdate;
        _timeEventChannel.OnDayChanged -= HandleDayChanged;
        _timeEventChannel.OnSeasonChanged -= HandleSeasonChanged;
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

    private void HandleGridUpdate(ItemModel itemModel, Vector3 pos)
    {
        Vector3Int cellPos = _currentGrid.WorldToCell(pos);
        string key = SceneManager.GetActiveScene().name+'&'+cellPos.x+'X'+cellPos.y+'Y';
        tileModelDict.TryGetValue(key, out TileModel tileModel);
        switch (itemModel.itemType)
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
            case ItemType.ReapTool:
            if (tileModel.seedItemID!=-1)
            {
                StaticItem staticItem = GetSatticItem(pos);
                if (staticItem==null)
                {
                    break;
                }
                if (staticItem.ProcessToolAction(itemModel.itemType))
                {
                    ClearDigGround(tileModel);
                }
            }
            break;
            case ItemType.Seed:
            _itemEventChannel.RaiseSelectItemDrop(1);
            _cropEventChannel.RaisePlantSeed(itemModel.itemID, tileModel);
            break;
        }
    }

    private void HandleDayChanged()
    {
        foreach (StaticItem staticItem in FindObjectsOfType<StaticItem>())
        {
            Destroy(staticItem.gameObject);
        }
        foreach (var elem in tileModelDict)
        {
            if (elem.Value.daysSinceDug > -1)
            {
                elem.Value.daysSinceDug++;
            }
            if (elem.Value.daysSinceWater > -1)
            {
                ClearWaterGround(elem.Value);
                Debug.Log("daychanged");
            }

            if (elem.Value.daysSinceDug > 4 && elem.Value.seedItemID == -1)
            {
                int p = UnityEngine.Random.Range(0, 10); 
                if (p <= elem.Value.daysSinceDug)
                {
                    ClearDigGround(elem.Value);
                }
            }
            if (elem.Value.growthDays > -1)
            {
                elem.Value.growthDays++;
                _cropEventChannel.RaisePlantSeed(-1, elem.Value);
            }
        }
    }
    
    private void HandleSeasonChanged(Season season)
    {
        _currentSeason = season;
    }
    #endregion

    private StaticItem GetSatticItem(Vector3 pos)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(pos);
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent<StaticItem>(out StaticItem staticItem))
            {
                return staticItem;
            }
        }
        return null;
    }

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

    #region 瓦片操作
    private void SetDigGround(TileModel tile)
    {
        Vector3Int pos = new(tile.gridX, tile.gridY, 0);
        if (_digTileMap != null)
        {
            _digTileMap.SetTile(pos, digTile);
            tile.daysSinceDug = 0;
        }
    }

    private void ClearDigGround(TileModel tile)
    {
        Vector3Int pos = new(tile.gridX, tile.gridY, 0);
        if (_digTileMap != null)
        {
            // 清除瓦片并重置天数
            _digTileMap.SetTile(pos, null);
            tile.daysSinceDug = -1; // -1 表示未挖掘状态
        }
    }


    private void SetWaterGround(TileModel tile)
    {
        Vector3Int pos = new(tile.gridX, tile.gridY, 0);
        if (_waterTileMap != null)
        {
            _waterTileMap.SetTile(pos, waterTile);
            tile.daysSinceWater = 0;
        }
    }

    private void ClearWaterGround(TileModel tile)
    {
        Vector3Int pos = new(tile.gridX, tile.gridY, 0);
        if (_waterTileMap != null)
        {
            _waterTileMap.SetTile(pos, null);
            tile.daysSinceWater = -1;
        }
    }

    private void UpdateGround()
    {
        // foreach (var elem in tileModelDict)
        // {
        //     if (elem.Value.daysSinceDug > -1)
        //     {
        //         elem.Value.daysSinceDug++;
        //     }
        //     if (elem.Value.daysSinceWater > -1)
        //     {
        //         elem.Value.daysSinceWater = -1;
        //     }

        //     if (elem.Value.daysSinceDug > 4 && elem.Value.seedItemID != -1)
        //     {
        //         elem.Value.daysSinceDug = -1;
        //     }
        // }
    }
    #endregion
}
