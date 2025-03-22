using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    [Header("参数")]
    [SerializeField] private Sprite normal, attack, tool, seed, item;

    [Header("组件获取")]
    [SerializeField] private UIAssetBridge _uiAssetBridge;

    [Header("事件通道")]
    [SerializeField] private ItemEventChannel _itemEventChannel;

    private Sprite _currentSprite;
    private Image _cursorImage;
    private Vector2 MousePos => InputManager.Instance.Controls.UI.Point.ReadValue<Vector2>();

    void OnEnable()
    {
        _itemEventChannel.OnItemSelect += HandleItemSelect;
    }

    void OnDisable()
    {
        _itemEventChannel.OnItemSelect -= HandleItemSelect;
    }

    private IEnumerator Start()
    {
        while (_uiAssetBridge == null)
        {
            yield return null;
        }
        _cursorImage = _uiAssetBridge.CursorImage;
        _currentSprite = normal;
        _cursorImage.sprite = _currentSprite;
    }

    private void Update()
    {
        if (_cursorImage == null) return;

        _cursorImage.transform.position = MousePos;

        if (!InteractWithUI())
        {
            _cursorImage.sprite = _currentSprite;
        }
        else
        {
            _cursorImage.sprite = normal;
        }
    }

    private void HandleItemSelect(ItemModel itemdata, bool isSelect)
    {
        if (!isSelect)
        {
            _currentSprite = normal;
        }
        else    //物品被选中才切换图片
        {
            //WORKFLOW:添加所有类型对应图片
            _currentSprite = itemdata.itemType switch
            {
                ItemType.Seed => seed,
                ItemType.Commodity => item,
                ItemType.ChopTool => tool,
                ItemType.HoeTool => tool,
                ItemType.WaterTool => tool,
                ItemType.BreakTool => tool,
                ItemType.ReapTool => tool,
                ItemType.Furniture => tool,
                _ => normal,
            };
        }
    }

    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
    }
}
