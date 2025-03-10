using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


[CreateAssetMenu(menuName = "Events/ItemDragChannel")]
public class ItemDragEventChannel : ScriptableObject
{
    public event UnityAction<int, Sprite, Vector2> OnBeginDrag;
    public event UnityAction OnDragUpdate;
    public event UnityAction<PointerEventData> OnEndDrag;

    public void RaiseBeginDrag(int slotIndex, Sprite dragSprite, Vector2 startPos) 
        => OnBeginDrag?.Invoke(slotIndex, dragSprite, startPos);

    public void RaiseDragUpdate() 
        => OnDragUpdate?.Invoke();

    public void RaiseEndDrag(PointerEventData eventData) 
        => OnEndDrag?.Invoke(eventData);
}