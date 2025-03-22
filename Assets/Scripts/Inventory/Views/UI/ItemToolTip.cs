using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ItemToolTip : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text typeText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Text value;

    private Vector2 MousePos => InputManager.Instance.Controls.UI.Point.ReadValue<Vector2>();

    public void UpdateTip(ItemModel itemData)
    {
        // InputManager.Instance.Controls.UI.Enable();
        nameText.text = itemData.itemName;
        typeText.text = TypeToString(itemData.itemType);
        descriptionText.text = itemData.itemDescription;
        value.text = ((int)(itemData.itemPrice * itemData.sellPercentage)).ToString();
        transform.position = MousePos + new Vector2(0, 40);
        // Debug.Log("MousePos=>" + MousePos);
        gameObject.SetActive(true);
    }

    private string TypeToString(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Seed => "种子",
            ItemType.Commodity => "商品",
            ItemType.Furniture => "家具",
            ItemType.HoeTool => "锄头",
            ItemType.ChopTool => "工具",
            ItemType.BreakTool => "工具",
            ItemType.ReapTool => "工具",
            ItemType.WaterTool => "水壶",
            ItemType.CollectTool => "镰刀",
            ItemType.PickAxe => "镐",
            _ => "",
        };
    }
}
