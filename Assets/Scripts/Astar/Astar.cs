using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar : MonoBehaviour
{
    [Header("资源引用")]
    [SerializeField] private List<MapDataSO> mapDataSOList = new();

    [Header("事件通道")]
    [SerializeField] private GridEventChannel _gridEventChannel;

    private GridNodes _gridNodes;
    private Node _startNode;
    private Node _endNode;
    private int _gridWidth;
    private int _gridHeight;
    private Vector2Int _originalPos;
    private string _sceneName;

    private List<Node> _openNodeList = new();
    private List<Node> _closeNodeList = new();
    private List<Vector2Int> _searchList = new(){
        new(0, 1),   // 上
        new(1, 0),   // 右
        new(0, -1),  // 下
        new(-1, 0)   // 左
    };
    private bool pathFound;

    public void BuildPath(string sceneName, Vector2Int startPos, Vector2Int endPos)
    {
        _sceneName = sceneName;
        foreach (var mapDataSO in mapDataSOList)
        {
            if (mapDataSO.sceneName == sceneName)
            {
                _gridWidth = mapDataSO.gridWidth;
                _gridHeight = mapDataSO.gridHeight;
                _gridNodes = new(_gridWidth, _gridHeight);
                _originalPos = mapDataSO.originalPos;
                _gridEventChannel.RaiseTileModelDictRequest(GenerateGridNodes);
                _startNode = _gridNodes.GetNode(startPos - _originalPos);
                _endNode = _gridNodes.GetNode(endPos - _originalPos);
                _startNode.gCost = 0;
                _startNode.hCost = GetL1Distance(startPos, endPos);
            }
        }
    }

    private void GenerateGridNodes(Dictionary<string, TileModel> tileModelDict)
    {
        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                string key = _sceneName+'&'+ x + _originalPos.x +'X'+ y + _originalPos.y +'Y';
                if (tileModelDict.TryGetValue(key, out TileModel tileModel))
                {
                    Node node = _gridNodes.GetNode(new Vector2Int(x, y));
                    node.isObstacle = tileModel.isNPCObstacle;
                }
            }
        }
    }

    public bool FindShortPath(out Stack<MovementStep> npcMovementStep)
    {

        _openNodeList.Clear();
        _closeNodeList.Clear();
        _openNodeList.Add(_startNode);
        while (_openNodeList.Count > 0)
        {
            _openNodeList.Sort();

            Node currentNode = _openNodeList[0];
            _openNodeList.RemoveAt(0);
            _closeNodeList.Add(currentNode);
            if (currentNode == _endNode)
            {
                pathFound = true;
                break;
            }
            EvaluateNeighbourNodes(currentNode);
        }
        if (pathFound)
        {
            Node currentNode = _endNode;
            npcMovementStep = new();
            while (currentNode != null)
            {
                npcMovementStep.Push(new(currentNode.gridPos + _originalPos));
                currentNode = currentNode.parent;
            }
        }
        else
        {
            npcMovementStep = null;
        }

        return pathFound;
    }

    private void EvaluateNeighbourNodes(Node currentNode)
    {
        foreach (var direction in _searchList)
        {
            Vector2Int neighborPos = currentNode.gridPos + direction;            
            if (_gridNodes.TryGetNode(neighborPos, out Node neighborNode))
            {
                // 跳过障碍物和已关闭节点
                if (neighborNode.isObstacle || _closeNodeList.Contains(neighborNode))
                    continue;

                // 正确计算gCost（当前节点gCost + 移动成本）
                int newGCost = currentNode.gCost + 1; 
                
                // 发现更优路径或新节点
                if (newGCost < neighborNode.gCost || !_openNodeList.Contains(neighborNode))
                {
                    neighborNode.gCost = newGCost;
                    neighborNode.hCost = GetL1Distance(neighborNode.gridPos, _endNode.gridPos);
                    neighborNode.parent = currentNode;

                    if (!_openNodeList.Contains(neighborNode))
                        _openNodeList.Add(neighborNode);
                }
            }
        }
    }

    private int GetL1Distance(Vector2Int startPos, Vector2Int endPos)
    {
        return Math.Abs(startPos.x - endPos.x) + Math.Abs(startPos.y - endPos.y);
    }
}
