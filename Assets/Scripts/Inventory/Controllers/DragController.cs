using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragController : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private Image dragImage;

    [Header("事件通道")]
    [SerializeField] private ItemDragEventChannel _itemDragEventChannel;
    [SerializeField] private ItemEventChannel _itemEventChannel;
    [SerializeField] private PlayerEventChannel _playerEventChannel;

    private Vector2 MousePos => InputManager.Instance.Controls.UI.Point.ReadValue<Vector2>();
    #region 拖拽slot的部分参数
    private int _slotIndex = -1;
    private int _itemID = -1;
    private int _quantity = 0;
    #endregion

    void OnEnable()
    {
        _itemDragEventChannel.OnBeginDrag += HandleBeginDrag;
        _itemDragEventChannel.OnDragUpdate += HandleDragUpdate;
        _itemDragEventChannel.OnEndDrag += HandleEndDrag;
    }

    void OnDisable()
    {
        _itemDragEventChannel.OnBeginDrag -= HandleBeginDrag;
        _itemDragEventChannel.OnDragUpdate -= HandleDragUpdate;
        _itemDragEventChannel.OnEndDrag -= HandleEndDrag;
    }

    public void HandleBeginDrag(ItemModel data, int quantity, int slotIndex, Vector2 startPos)
    {
        dragImage.gameObject.SetActive(true);
        _slotIndex = slotIndex;
        _itemID = data.itemID;
        _quantity = quantity;
        dragImage.sprite = data.itemIcon;
        dragImage.transform.position = startPos;

    }

    public void HandleDragUpdate()
    {
        dragImage.transform.position = MousePos;
    }

    public void HandleEndDrag(PointerEventData eventData)
    {
        dragImage.sprite = null;
        dragImage.gameObject.SetActive(false);
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent<ItemSlot>(out ItemSlot itemSlot))
            {
                _itemEventChannel.RaiseItemSwap(_slotIndex, itemSlot._slotIndex);
            }
        }
        else
        {
            _itemEventChannel.RaiseItemDrop(_slotIndex, _quantity);
            _itemEventChannel.RaiseCreateWorldItemWithVelocity(_itemID, _quantity, Camera.main.ScreenToWorldPoint(MousePos));
            _playerEventChannel.RaiseHoldItem(null, false);
            _itemEventChannel.RasieItemSelect(null, false);
        }
    }
}
