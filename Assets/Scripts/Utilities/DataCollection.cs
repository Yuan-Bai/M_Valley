// using UnityEngine;

// [System.Serializable]
// public class ItemDetails
// {
//     public int itemID;
//     public string itemName;
//     public ItemType itemType;
//     public Sprite itemIcon;
//     public Sprite itemOnWorldIcon;
//     public string itemDescription;
//     public int itemUseRadius;
//     public bool canPickedup;
//     public bool canDropped;
//     public bool canCarried;
//     public int itemPrice;
//     [Range(0, 1)]
//     public float sellPercentage;
// }


// [System.Serializable]
// public struct ItemVisuals
// {
//     [Tooltip("背包内显示的图标")]
//     public Sprite inventoryIcon;

//     [Tooltip("世界地图上显示的图标")]
//     public Sprite worldIcon;
    
//     [Tooltip("角色手持时的模型预制体")]
//     public GameObject heldPrefab;
// }


// [System.Serializable]
// public class ItemDetails
// {
//     [Header("基本信息")]
//     public int itemID; // 使用自定义PropertyDrawer
//     public string itemName;
//     public ItemCategory category;
    
//     [Header("视觉表现")]
//     public ItemVisuals visuals;
    
//     [TextArea(3, 5)] 
//     public string description;
    
//     [Header("交互属性")]
//     public ItemAttributes attributes;
    
//     [Header("经济系统")]
//     [Range(0, 1000)] 
//     public int basePrice;
    
//     [Tooltip("出售价格系数 (0-1)")]
//     [Range(0, 1)] 
//     public float sellMultiplier = 0.8f;
    
//     [Header("工具属性")]
//     public ToolType toolType;
//     [Range(1, 5)] 
//     public int toolLevel = 1;
//     [Min(0)] 
//     public float useRadius = 1.0f;

//     public int SellPrice => Mathf.RoundToInt(basePrice * sellMultiplier);
// }