using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorOverrider : MonoBehaviour
{
    private Animator[] _animators;
    [SerializeField] private SpriteRenderer _itemHolder;
    [SerializeField] private PlayerEventChannel _playerEventChannel;
    
    [Header("动画参数")]
    [SerializeField] private List<AnimatorType> _animatorTypes;
    
    private Dictionary<string, Animator> _animatorDict = new();

    void Awake()
    {
        _animators = GetComponentsInChildren<Animator>();
        foreach(var animator in _animators)
        {
            _animatorDict.Add(animator.name, animator);
        }
        _itemHolder.enabled = false;
    }

    void OnEnable()
    {
        _playerEventChannel.OnHoldItem += HandleHoldItem;
    }

    void OnDisable()
    {
        _playerEventChannel.OnHoldItem -= HandleHoldItem;
    }

    private void HandleHoldItem(ItemModel itemData, bool isSelect)
    {
        // TODO
        if (itemData==null)
        {
            _itemHolder.enabled = false;
            SwitchAnimator(ActionType.None);
            return;
        }
        ActionType currentActionType = itemData.itemType switch
        {
            ItemType.Seed => ActionType.Hold,
            ItemType.Commodity => ActionType.Hold,
            ItemType.HoeTool => ActionType.Hoe,
            ItemType.WaterTool => ActionType.Water,
            ItemType.ReapTool => ActionType.Reap,
            ItemType.ChopTool => ActionType.Axe,
            _ => ActionType.None,
        };
        if (!isSelect)
        {
            currentActionType = ActionType.None;
            _itemHolder.enabled = false;
        }
        else
        {
            if (currentActionType == ActionType.Hold)
            {
                _itemHolder.enabled = true;
                _itemHolder.sprite = itemData.itemOnWorldIcon == null ? itemData.itemIcon:itemData.itemOnWorldIcon;
            }
            else
            {
                _itemHolder.enabled = false;
            }
        }
        SwitchAnimator(currentActionType);
    }

    private void SwitchAnimator(ActionType currentActionType)
    {
        foreach (var animatorType in _animatorTypes)
        {
            if (animatorType.actionType == currentActionType)
            {
                _animatorDict[animatorType.partType.ToString()].runtimeAnimatorController = animatorType.overrideController;
            }
        }
    }
}


[System.Serializable]
public class AnimatorType
{
    public ActionType actionType;
    public PartType partType;
    public AnimatorOverrideController overrideController;
}
