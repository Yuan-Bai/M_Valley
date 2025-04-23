using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Vector2Int gridPos;
    public int gCost;  // 距离startPos的距离
    public int hCost;  // 距离endPos的距离
    public int FCost => gCost + hCost;  //当前节点的代价
    public Node parent;

    public bool isObstacle;

    public Node(Vector2Int gridPos)
    {
        parent = null;
        this.gridPos = gridPos;
    }

    public Node(Node parent, Vector2Int gridPos)
    {
        this.parent = parent;
        this.gridPos = gridPos;
    }

    public int CompareTo(Node other)
    {
        //比较选出最低的F值，返回-1，0，1
        int result = FCost.CompareTo(other.FCost);
        return result == 0 ? hCost.CompareTo(other.hCost) : result;
    }
}
