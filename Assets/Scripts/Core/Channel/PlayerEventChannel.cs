using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "PlayerEventChannel", menuName = "Events/PlayerEventChannel")]
public class PlayerEventChannel : ScriptableObject
{
    public event UnityAction<ItemModel, bool> OnHoldItem;

    public event UnityAction<ItemModel, Vector2> OnToolAnimation;

    public void RaiseHoldItem(ItemModel itemData, bool isSelect)
    {
        OnHoldItem?.Invoke(itemData, isSelect);
    }

    public void RaiseToolAnimation(ItemModel itemModel, Vector2 pos)
    {
        OnToolAnimation?.Invoke(itemModel, pos);
    }
}