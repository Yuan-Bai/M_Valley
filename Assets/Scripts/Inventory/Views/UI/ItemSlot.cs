using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class ItemSlot : MonoBehaviour
{
    [Header("组件获取")]
    [SerializeField] private Image _icon;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private TMP_Text _countText;
    [SerializeField] private Button _button;

    private int _slotIndex;
    private Action<int> _onClickAction;

    public void Initialize(int index, Action<int> onClick) {
        _slotIndex = index;
        _onClickAction = onClick;
        _button.onClick.AddListener(() => _onClickAction?.Invoke(_slotIndex));
        Clear();
    }

    public void UpdateItem(ItemModel data, int count) {
        _icon.sprite = data.itemIcon;
        _countText.text = count > 1 ? count.ToString() : "";
        _icon.gameObject.SetActive(true);
    }

    public void SetHighlight(bool state) {
        _highlight.SetActive(state);
    }

    public void Clear() {
        _icon.gameObject.SetActive(false);
        _countText.text = "";
        SetHighlight(false);
    }
}
