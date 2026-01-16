using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Direction
{
    [SerializeField]
    int direction = 1;

    public Direction(int direction)
    {
        this.direction = direction;
    }

    public static implicit operator int(Direction direction)
    {
        return direction.direction;
    }
    public static implicit operator Direction(int direction)
    {
        return new Direction(direction);
    }
}
