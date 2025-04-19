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
        _itemEventChannel.OnCreateItem += HandleCreateItem;
    }

    void OnDisable()
    {
        _sceneEventChannel.OnBeforeSceneUnload -= HandleBeforeSceneUnload;
        _sceneEventChannel.OnAfterSceneLoad -= HandleAfterSceneLoad;
        _itemEventChannel.OnCreateWorldItemWithVelocity -= HandleCreateWorldItemWithVelocity;
        _itemEventChannel.OnCreateItem -= HandleCreateItem;
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

    #region 事件
    private void HandleBeforeSceneUnload()
    {
        SaveAllItemsBeforeSceneUnload();
    }

    private void HandleAfterSceneLoad()
    {
        _worldItemParent = GameObject.FindWithTag("WordItemParent")?.transform;
        RecoverAllItemsAfterSceneLoad();
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

    private void HandleCreateItem(int itemID, Vector2 pos, int radius, int quantity)
    {
        Vector2 startPos = GenerateRandomPoint(pos, radius);
        var item = Instantiate(_itemPrefab, startPos, Quaternion.identity, _worldItemParent);
        item.SetItemID(itemID);
        item.quantity = quantity;
    }
    #endregion

    private void CreateWorldItem(SceneItem sceneItem)
    {
        var item = Instantiate(_itemPrefab, sceneItem.pos.ToVector3(), Quaternion.identity, _worldItemParent);
        item.SetItemID(sceneItem.itemID);
        item.quantity = sceneItem.quantity;
    }

    private Vector2 GenerateRandomPoint(Vector2 pos, int radius)
    {
        float randomRadius = Mathf.Sqrt(Random.Range(0f, 1f)) * radius;
        float randomAngle = Random.Range(0f, Mathf.PI*2);
        float xOffset = randomRadius*Mathf.Cos(randomAngle);
        float yOffset = randomRadius*Mathf.Sin(randomAngle);
        return new Vector2(pos.x+xOffset, pos.y+yOffset);
    }

}
