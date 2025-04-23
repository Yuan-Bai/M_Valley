using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNodes
{
    private int _width;
    private int _height;
    private Node[,] _nodes;


    public GridNodes(int width, int height)
    {
        _width = width;
        _height = height;
        _nodes = new Node[_width, _height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                _nodes[i, j] = new Node(new Vector2Int(i, j));
            }
        }
    }

    public Node GetNode(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x > _width || pos.y < 0 || pos.y > _height) return null;
        if (_nodes[pos.x, pos.y].isObstacle) return null;
        return _nodes[pos.x, pos.y];
    }

    public bool TryGetNode(Vector2Int pos, out Node node)
    {
        if (pos.x < 0 || pos.x > _width || pos.y < 0 || pos.y > _height)
        {
            node = null;
            return false;
        }
        else
        {
            node = _nodes[pos.x, pos.y];
            if (node.isObstacle)
            {
                return false;
            }
            return true;
        }
    }
}
