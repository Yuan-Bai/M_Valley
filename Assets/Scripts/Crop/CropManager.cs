using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropManager : MonoBehaviour
{
    [Header("资源获取")]
    [SerializeField] private CropDataBase_SO _cropDataBase;
    
    [Header("事件通道")]
    [SerializeField] private SceneEventChannel _sceneEventChannel;
    [SerializeField] private TimeEventChannel _timeEventChannel;
    [SerializeField] private CropEventChannel _cropEventChannel;

    private Transform _cropParent;
    private Season _currentSeason;


    void OnEnable()
    {
        _sceneEventChannel.OnAfterSceneLoad += HandleAfterSceneLoad;
        _timeEventChannel.OnSeasonChanged += HandleSeasonChanged;
        _cropEventChannel.OnPlantSeed += HandlePlantSeed;
    }

    void OnDisable()
    {
        _sceneEventChannel.OnAfterSceneLoad -= HandleAfterSceneLoad;
        _timeEventChannel.OnSeasonChanged -= HandleSeasonChanged;
        _cropEventChannel.OnPlantSeed -= HandlePlantSeed;
    }

    #region 事件
    private void HandleAfterSceneLoad()
    {
        _cropParent = GameObject.FindWithTag("CropParent")?.transform;
    }

    private void HandleSeasonChanged(Season season)
    {
        _currentSeason = season;
    }

    private void HandlePlantSeed(int itemId, TileModel tileModel)
    {
        CropModel currentCrop = _cropDataBase.GetCropModelByID(itemId);
        if (currentCrop != null && SeasonAvailable(currentCrop) && tileModel.seedItemID == -1)    //用于第一次种植
        {
            tileModel.seedItemID = itemId;
            tileModel.growthDays = 0;
            //显示农作物
            DisplayCropPlant(tileModel, currentCrop);
        }
        else if (tileModel.seedItemID != -1)  //用于刷新地图
        {
            //显示农作物
            DisplayCropPlant(tileModel, _cropDataBase.GetCropModelByID(tileModel.seedItemID));
        }
    }
    #endregion

    private void DisplayCropPlant(TileModel tileModel, CropModel cropModel)
    {
        int[] growthDays = cropModel.growthDays;
        int currentStage = 0;
        int accumulatedDays = 0;

        // 遍历每个阶段的天数要求
        for (int i = 0; i < growthDays.Length; i++) 
        {
            accumulatedDays += growthDays[i];
            
            // 如果当前生长天数未达到本阶段累计要求
            if (tileModel.growthDays < accumulatedDays)
            {
                currentStage = i;
                break;
            }
            
            // 如果已处理完所有定义阶段，则属于最后阶段
            currentStage = i + 1;
        }

        //获取当前阶段的Prefab
        GameObject cropPrefab = cropModel.growthPrefabs[currentStage];
        Sprite cropSprite = cropModel.growthSprites[currentStage];

        Vector3 pos = new(tileModel.gridX + 0.5f, tileModel.gridY + 0.5f, 0);

        GameObject cropInstance = Instantiate(cropPrefab, pos, Quaternion.identity, _cropParent);
        StaticItem staticItem = cropInstance.GetComponent<StaticItem>();
        staticItem.SetSprite(cropSprite);
        staticItem.UpdateCollider();
        staticItem.Initialize(cropModel);
        staticItem.harvestable = currentStage >= growthDays.Length;
    }

    private bool SeasonAvailable(CropModel cropModel)
    {
        foreach (Season season in cropModel.seasons)
        {
            if (season == _currentSeason)
            {
                return true;
            }
        }
        return false;
    }
}
