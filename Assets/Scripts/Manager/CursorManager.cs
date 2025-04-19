using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class CursorManager : MonoBehaviour
{
    [Header("参数")]
    [SerializeField] private Color _inValidColor = new Color(1, 0, 0, 0.4f);
    [SerializeField] private Color _validColor = new Color(1, 1, 1, 1);
    [SerializeField] private Sprite _normal, _attack, _tool, _seed, _item;

    [Header("组件获取")]
    [SerializeField] private UIAssetBridge _uiAssetBridge;
    [SerializeField] private Transform _player;

    [Header("事件通道")]
    [SerializeField] private ItemEventChannel _itemEventChannel;
    [SerializeField] private SceneEventChannel _sceneEventChannel;
    [SerializeField] private GridEventChannel _gridEventChannel;
    [SerializeField] private PlayerEventChannel _playerEventChannel;

    #region 外观私有变量
    private Sprite _currentSprite;
    private Image _cursorImage;
    #endregion

    #region 逻辑私有变量
    private Grid _currentGrid;
    private ItemModel _itemModel;
    #endregion
    private Vector2 MousePos => InputManager.Instance.Controls.UI.Point.ReadValue<Vector2>();
    private Vector3 WorldMousePos;
    private Vector3Int CellMousePos;
    private Vector3Int CellPlayerPos;

    void OnEnable()
    {
        StartCoroutine(DelayedSubscribe());
        _itemEventChannel.OnItemSelect += HandleItemSelect;
        _sceneEventChannel.OnAfterSceneLoad += HandleAfterSceneLoad;
    }

    void OnDisable()
    {
        if (InputManager.IsAvailable)
        {
            InputManager.Instance.Controls.UI.Click.performed -= OnClickPerformed;
            InputManager.Instance.Controls.UI.Disable();
        }
        _itemEventChannel.OnItemSelect -= HandleItemSelect;
        _sceneEventChannel.OnAfterSceneLoad -= HandleAfterSceneLoad;
    }

    private IEnumerator DelayedSubscribe()
    {
        yield return new WaitUntil(() => InputManager.IsAvailable);
        InputManager.Instance.Controls.UI.Enable();
        InputManager.Instance.Controls.UI.Click.performed += OnClickPerformed;
    }

    private IEnumerator Start()
    {
        while (_uiAssetBridge == null)
        {
            yield return null;
        }
        _cursorImage = _uiAssetBridge.CursorImage;
        _currentSprite = _normal;
        _cursorImage.sprite = _currentSprite;
    }

    private void Update()
    {
        if (_cursorImage == null) return;

        _cursorImage.transform.position = MousePos;

        if (_itemModel != null)
        {
            if (CheckMouseVaild(_itemModel.itemUseRadius))
            {
                SetMouseColor(_validColor);
            }
            else
            {
                SetMouseColor(_inValidColor);
            }
        }
        else
        {
            SetMouseColor(_validColor);
        }

        if (!InteractWithUI())
        {
            _cursorImage.sprite = _currentSprite;
        }
        else
        {
            _cursorImage.sprite = _normal;
        }
    }

    #region 事件
    private void OnClickPerformed(CallbackContext context)
    {
        // 只允许鼠标按下后释放时触发
        if (context.ReadValue<float>() > 0.5f)
        {
            return;
        }
        
        if (_itemModel == null || _cursorImage == null) return;
        if (CheckMouseVaild(_itemModel.itemUseRadius))
        {
            _playerEventChannel.RaiseToolAnimation(_itemModel, WorldMousePos);
        }
    }

    private void HandleItemSelect(ItemModel itemdata, bool isSelect)
    {
        if (!isSelect)
        {
            _currentSprite = _normal;
            _itemModel = null;
        }
        else    //物品被选中才切换图片
        {
            //WORKFLOW:添加所有类型对应图片
            _currentSprite = itemdata.itemType switch
            {
                ItemType.Seed => _seed,
                ItemType.Commodity => _item,
                ItemType.ChopTool => _tool,
                ItemType.HoeTool => _tool,
                ItemType.WaterTool => _tool,
                ItemType.BreakTool => _tool,
                ItemType.ReapTool => _tool,
                ItemType.Furniture => _tool,
                _ => _normal,
            };
            _itemModel = itemdata;
        }
    }

    private void HandleAfterSceneLoad()
    {
        _currentGrid = FindAnyObjectByType<Grid>();
    }
    #endregion

    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
    }

    private bool CheckMouseVaild(int itemUseRadius)
    {
        WorldMousePos = Camera.main.ScreenToWorldPoint(MousePos);
        CellMousePos = _currentGrid.WorldToCell(WorldMousePos);

        CellPlayerPos = _currentGrid.WorldToCell(_player.position);
        return Mathf.Abs(CellMousePos.x-CellPlayerPos.x) + Mathf.Abs(CellMousePos.y-CellPlayerPos.y) <= itemUseRadius;
    }

    private void SetMouseColor(Color color)
    {
        _cursorImage.color = color;
    }
}
