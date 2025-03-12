using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(ItemSlot))]
public class ShowItemToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    [Header("事件")]
    [SerializeField] private InventoryEventChannel _channel;

    private ItemSlot _itemSlot;


    void Awake()
    {
        _itemSlot = GetComponent<ItemSlot>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_itemSlot._itemData == null) return;
        _channel.RaiseTipUpdate(_itemSlot._itemData, false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_itemSlot._itemData == null) return;
        _channel.RaiseTipUpdate(_itemSlot._itemData, true);
    }
}
