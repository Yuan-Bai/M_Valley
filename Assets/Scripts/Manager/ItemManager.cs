using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private WorldItem _itemPrefab;
    [SerializeField] private Transform _player;
    [SerializeField] private SceneEventChannel _sceneEventChannel;
    [SerializeField] private ItemEventChannel _itemEventChannel;
    private Transform _worldItemParent;

    void OnEnable()
    {
        _sceneEventChannel.OnAfterSceneLoad += HandleAfterSceneLoad;
        _itemEventChannel.OnCreateWorldItemWithVelocity += HandleCreateWorldItemWithVelocity;
    }

    void OnDisable()
    {
        _sceneEventChannel.OnAfterSceneLoad -= HandleAfterSceneLoad;
        _itemEventChannel.OnCreateWorldItemWithVelocity -= HandleCreateWorldItemWithVelocity;
    }

    private void HandleAfterSceneLoad()
    {
        _worldItemParent = GameObject.FindWithTag("WordItemParent")?.transform;
    }

    private void HandleCreateWorldItemWithVelocity(int itemID, int quantity, Vector2 targetPos)
    {
        Vector2 startPos = _player.position;
        var item = Instantiate(_itemPrefab, startPos, Quaternion.identity, _worldItemParent);
        item.SetItemID(itemID);
        item.quantity = quantity;
        StartCoroutine(item.SetColliderDisableForSeconds(1.5f));
        item.SetVelocity((targetPos-startPos).normalized * 7);
    }
}
