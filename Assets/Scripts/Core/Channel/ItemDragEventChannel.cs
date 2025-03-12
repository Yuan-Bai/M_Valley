using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


[CreateAssetMenu(menuName = "Events/ItemDragChannel")]
public class ItemDragEventChannel : ScriptableObject
{
    public event UnityAction<ItemModel, int, int, Vector2> OnBeginDrag;
    public event UnityAction OnDragUpdate;
    public event UnityAction<PointerEventData> OnEndDrag;

    public void RaiseBeginDrag(ItemModel data, int quantity, int slotIndex, Vector2 startPos) 
        => OnBeginDrag?.Invoke(data, quantity, slotIndex, startPos);

    public void RaiseDragUpdate() 
        => OnDragUpdate?.Invoke();

    public void RaiseEndDrag(PointerEventData eventData) 
        => OnEndDrag?.Invoke(eventData);
}