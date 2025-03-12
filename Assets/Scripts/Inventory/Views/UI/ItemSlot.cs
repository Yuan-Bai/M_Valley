using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("组件获取")]
    [SerializeField] private Image _icon;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private TMP_Text _countText;
    [SerializeField] private Button _button;
    [SerializeField] private ItemDragEventChannel _dragEventChannel;

    public int _slotIndex;
    private int _itemCount = 0;
    public ItemModel _itemData;
    private Action<int> _onClickAction;

    public void Initialize(int index, Action<int> onClick) {
        _slotIndex = index;
        _onClickAction = onClick;
        _button.onClick.AddListener(() => _onClickAction?.Invoke(_slotIndex));
        Clear();
    }

    public void UpdateItem(ItemModel data, int count) {
        _icon.sprite = data.itemIcon;
        _itemCount = count;
        _countText.text = count > 1 ? count.ToString() : "";
        _itemData = data;
        _icon.gameObject.SetActive(true);
    }

    public void SetHighlight(bool state) {
        _highlight.SetActive(state);
    }

    public void Clear() {
        _icon.gameObject.SetActive(false);
        _itemCount = 0;
        _countText.text = "";
        _itemData = null;
        SetHighlight(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_itemCount == 0) return;
        _dragEventChannel.RaiseBeginDrag(_itemData, _itemCount, _slotIndex, transform.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_itemCount == 0) return;
        _dragEventChannel.RaiseDragUpdate();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_itemCount == 0) return;
        _dragEventChannel.RaiseEndDrag(eventData);
    }
}
