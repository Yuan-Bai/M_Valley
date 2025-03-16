using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "PlayerEventChannel", menuName = "Events/PlayerEventChannel")]
public class PlayerEventChannel : ScriptableObject
{
    public event UnityAction<ItemModel, bool> OnHoldItem;

    public void RaiseHoldItem(ItemModel itemData, bool isSelect)
    {
        OnHoldItem?.Invoke(itemData, isSelect);
    }
}