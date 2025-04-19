using UnityEngine;

[System.Serializable]
public class CropModel
{
    public int seedItemID;

    [Header("不同阶段成长天数")]
    public int[] growthDays;
    public int TotalGrowthDays
    {
        get
        {
            int amount = 0;
            foreach (int growthDay in growthDays)
                amount += growthDay;
            return amount;
        }
    }

    [Header("不同生长阶段的prefab")]
    public GameObject[] growthPrefabs;
    [Header("不同阶段的图片")]
    public Sprite[] growthSprites;
    [Header("可种植的季节")]
    public Season[] seasons;

    [Space]
    [Header("收割工具")]
    public ItemType[] harvestToolType;
    [Header("使用次数")]
    public int requireActionCount;
    [Header("转换新物品ID")]
    public int transferItemID = -1;

    [Space]
    [Header("收割果实信息")]
    public int[] producedItemID;
    public int[] producedMinAmount;
    public int[] producedMaxAmount;
    public Vector2 spawnRadius;

    [Header("再次生长时间")]
    public int daysToRegrow;
    public int regrowTimes;

    [Header("Options")]
    public bool generateAtPlayerPosition;
    public bool hasAnimation;
    public bool hasParticalEffect;

}
