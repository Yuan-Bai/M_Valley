using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStep
{
    public Vector2Int gridCoordinate;

    public MovementStep(Vector2Int gridCoordinate)
    {
        this.gridCoordinate = gridCoordinate;
    }
}
