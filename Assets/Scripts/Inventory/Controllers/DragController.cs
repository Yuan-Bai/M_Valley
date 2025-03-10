using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragController : MonoBehaviour
{
    [SerializeField] private Image dragImage;
    [SerializeField] private ItemDragEventChannel _itemDragEventChannel;
    [SerializeField] private ItemEventChannel _itemEventChannel;

    private Vector2 _mousePos => InputManager.Instance.Controls.UI.Point.ReadValue<Vector2>();
    private int _slotIndex = -1;

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

    public void HandleBeginDrag(int slotIndex, Sprite dragSprite, Vector2 startPos)
    {
        dragImage.gameObject.SetActive(true);
        _slotIndex = slotIndex;
        dragImage.sprite = dragSprite;
        dragImage.transform.position = startPos;

    }

    public void HandleDragUpdate()
    {
        dragImage.transform.position = _mousePos;
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
    }
}
