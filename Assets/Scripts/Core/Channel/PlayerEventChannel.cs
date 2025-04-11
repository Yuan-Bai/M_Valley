using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "PlayerEventChannel", menuName = "Events/PlayerEventChannel")]
public class PlayerEventChannel : ScriptableObject
{
    public event UnityAction<ItemModel, bool> OnHoldItem;

    public event UnityAction<ItemType, Vector2> OnToolAnimation;

    public void RaiseHoldItem(ItemModel itemData, bool isSelect)
    {
        OnHoldItem?.Invoke(itemData, isSelect);
    }

    public void RaiseToolAnimation(ItemType itemType, Vector2 pos)
    {
        OnToolAnimation?.Invoke(itemType, pos);
    }
}