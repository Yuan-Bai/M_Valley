using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class AstarTest : MonoBehaviour
{
    [SerializeField] private Vector2Int _startPos;
    [SerializeField] private Vector2Int _endPos;
    [SerializeField] private Tilemap _displayMap;
    [SerializeField] private TileBase _displayTile;
    [SerializeField] private bool _isDisplayStartAndEnd;
    [SerializeField] private bool _isDisplayPath;

    [Header("事件通道")]
    [SerializeField] private SceneEventChannel _sceneEventChannel;

    private Astar _astar;
    private string _sceneName;

    void Awake()
    {
        _astar = GetComponent<Astar>();
        if (_astar == null)
        {
            Debug.Log("astar is null");
        }
    }

    void OnEnable()
    {
        _sceneEventChannel.OnAfterSceneLoad += HandleSceneLoad;
    }

    void OnDisable()
    {
        _sceneEventChannel.OnAfterSceneLoad -= HandleSceneLoad;
    }

    void Update()
    {
        DisplayAstarRes();
    }

    #region 事件
    private void HandleSceneLoad()
    {
        _sceneName = SceneManager.GetActiveScene().name;
        _displayMap = GameObject.FindWithTag("AstarTest").GetComponent<Tilemap>();
    }
    #endregion

    private void DisplayAstarRes()
    {
        if (_displayMap != null && _displayTile != null)
        {
            if (!_isDisplayPath) return;
            _isDisplayPath = !_isDisplayPath;
            _astar.BuildPath(_sceneName, _startPos, _endPos);
            if (_astar.FindShortPath(out Stack<MovementStep> npcMovementStep))
            {
                foreach(var step in npcMovementStep)
                {
                    _displayMap.SetTile((Vector3Int)step.gridCoordinate, _displayTile);
                }
            }
        }
    }
}
