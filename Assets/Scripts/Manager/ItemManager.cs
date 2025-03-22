using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private WorldItem _itemPrefab;
    [SerializeField] private Transform _player;
    [SerializeField] private SceneEventChannel _sceneEventChannel;
    [SerializeField] private ItemEventChannel _itemEventChannel;

    [Header("参数")]
    [SerializeField] private float _gravity;
    [SerializeField] private float _velocityZ;
    [SerializeField] private float _height;
    
    private Transform _worldItemParent;
    // sceneName=>sceneItemList
    private Dictionary<string, List<SceneItem>> _sceneItemDict = new();

    void OnEnable()
    {
        _sceneEventChannel.OnBeforeSceneUnload += HandleBeforeSceneUnload;
        _sceneEventChannel.OnAfterSceneLoad += HandleAfterSceneLoad;
        _itemEventChannel.OnCreateWorldItemWithVelocity += HandleCreateWorldItemWithVelocity;
    }

    void OnDisable()
    {
        _sceneEventChannel.OnBeforeSceneUnload -= HandleBeforeSceneUnload;
        _sceneEventChannel.OnAfterSceneLoad -= HandleAfterSceneLoad;
        _itemEventChannel.OnCreateWorldItemWithVelocity -= HandleCreateWorldItemWithVelocity;
    }

    private void HandleBeforeSceneUnload()
    {
        SaveAllItemsBeforeSceneUnload();
    }

    private void HandleAfterSceneLoad()
    {
        _worldItemParent = GameObject.FindWithTag("WordItemParent")?.transform;
        RecoverAllItemsAfterSceneLoad();
    }

    private void SaveAllItemsBeforeSceneUnload()
    {
        // 获取当前场景的所有worlditem
        List<SceneItem> currentSceneItemList = new();
        foreach (var item in FindObjectsOfType<WorldItem>(false))
        {
            currentSceneItemList.Add(new SceneItem
            {
                itemID = item.ItemID,
                pos = new SerializableVector3(item.transform.position),
                quantity = item.quantity,
            });
        }
        // 判断dict的key中是否存在当前场景，存在更新，不在添加
        string activeSceneName = SceneManager.GetActiveScene().name;
        if (_sceneItemDict.ContainsKey(activeSceneName))
        {
            _sceneItemDict[activeSceneName] = currentSceneItemList;
        }
        else
        {
            _sceneItemDict.Add(activeSceneName, currentSceneItemList);
        }
    }

    private void RecoverAllItemsAfterSceneLoad()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;

        if (_sceneItemDict.TryGetValue(activeSceneName, out var sceneItemList))
        {
            //清场
            foreach (var item in FindObjectsOfType<WorldItem>(false))
            {
                Destroy(item.gameObject);
            }
            foreach (var item in sceneItemList)
            {
                CreateWorldItem(item);
            }
        }
    }

    private void HandleCreateWorldItemWithVelocity(int itemID, int quantity, Vector2 targetPos)
    {
        Vector2 startPos = new(_player.position.x, _player.position.y+_height);
        float deltaX = targetPos.x - _player.position.x;
        float deltaY = (targetPos.y - _player.position.y)/Mathf.Cos(Mathf.PI/4);
        float totalTime = (_velocityZ + Mathf.Sqrt(_velocityZ*_velocityZ+2*_gravity*_height/Mathf.Cos(Mathf.PI/4)))/_gravity;

        float vx = deltaX / totalTime;
        float vy = deltaY / totalTime;

        var item = Instantiate(_itemPrefab, startPos, Quaternion.identity, _worldItemParent);
        item.SetItemID(itemID);
        item.quantity = quantity;
        // 执行丢弃
        item.Throw(new Vector3(vx, vy, _velocityZ), totalTime, _gravity, _player.GetComponent<CircleCollider2D>());
    }

    private void CreateWorldItem(SceneItem sceneItem)
    {
        var item = Instantiate(_itemPrefab, sceneItem.pos.ToVector3(), Quaternion.identity, _worldItemParent);
        item.SetItemID(sceneItem.itemID);
        item.quantity = sceneItem.quantity;
    }

}
