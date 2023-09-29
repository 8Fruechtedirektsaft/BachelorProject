using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//pointy top hexagon
public static class HexGridHelper 
{
    public static Vector3 GridToWorld(Vector2Int gridPosition)
    {
        Vector3 worldPosition = Vector3.zero;
        worldPosition.x = gridPosition.x * horizontalSpacing + 0.5f * horizontalSpacing * gridPosition.y;
        worldPosition.z = (-1) * gridPosition.y * verticalSpacing;
        return worldPosition;
    }

    public static Vector2Int[] GetNeighboorPositions(Vector2Int position)
    {
        Vector2Int[] neighboorPositions = {
            new (position.x + 1, position.y), new (position.x + 1, position.y - 1), new (position.x, position.y - 1),
            new (position.x - 1, position.y), new (position.x - 1, position.y + 1), new (position.x, position.y + 1)
        };
        return neighboorPositions;
    }

    public static Vector2Int GetOppositeDirection(Vector2Int direction)
    {
        Vector2Int oppositeDirection = direction * -1;
        return oppositeDirection;
    }

    public static int GetClockRotationsBetween(Vector2Int firstDirection, Vector2Int secondDirection)
    {
        Vector2Int clockDirection = firstDirection;
        for (int i = 1; i < 6; i++)
        {
            clockDirection = GetNeighboorClockwise(clockDirection);
            if (clockDirection == secondDirection)
            {
                return i;
            }
        }
        return 0;
    }

    public static List<Vector2Int> GetGeneralDirections(Vector2Int direction)
    {
        List<Vector2Int> generalDirections = new();
        generalDirections.Add(direction);
        generalDirections.Add(GetNeighboorClockwise(direction));
        generalDirections.Add(GetNeighboorCounterClockwise(direction));
        return generalDirections;
    }

    public static Vector2Int GetNeighboorClockwise(Vector2Int position)
    {
        return new(-position.y, position.x + position.y);
    }

    public static Vector2Int GetNeighboorClockwise(Vector2Int position, int clockTicks)
    {
        Vector2Int newPosition = position;
        for (int t = 0; t < clockTicks; t++)
        {
            newPosition = GetNeighboorClockwise(newPosition);
        }
        return newPosition;
    }

    public static Vector2Int GetNeighboorCounterClockwise(Vector2Int position)
    {
        return new(position.x + position.y, -position.x);
    }

    public static Vector2Int GetNeighboorCounterClockwise(Vector2Int position, int clockTicks)
    {
        Vector2Int newPosition = position;
        for (int t = 0; t < clockTicks; t++)
        {
            newPosition = GetNeighboorCounterClockwise(newPosition);
        }
        return newPosition;
    }

    public static Vector2Int[] GetNeighboorPositionsRadiusTwo(Vector2Int position)
    {
        Vector2Int[] neighboorPositions = {
            new(position.x + 1, position.y), new (position.x + 1, position.y - 1), new (position.x, position.y - 1),
            new (position.x - 1, position.y), new (position.x - 1, position.y + 1), new (position.x, position.y + 1),
            new (position.x + 1, position.y - 2), new (position.x + 2, position.y - 2), new (position.x + 2, position.y - 1),
            new (position.x + 2, position.y), new (position.x + 1, position.y + 1), new (position.x, position.y + 2),
            new (position.x - 1, position.y + 2), new (position.x - 2, position.y + 2), new (position.x - 2, position.y + 1),
            new (position.x - 2, position.y), new (position.x - 1, position.y - 1), new (position.x, position.y - 2),
        };
        return neighboorPositions;
    }

    public static Vector2Int DirectionNE
    {
        get
        {
            return new Vector2Int(1, -1);
        }
    }

    public static Vector2Int DirectionSE
    {
        get
        {
            return new Vector2Int(0, 1);
        }
    }

    public static Vector2Int DirectionNW
    {
        get
        {
            return new Vector2Int(0, -1);
        }
    }

    public static Vector2Int DirectionSW
    {
        get
        {
            return new Vector2Int(-1, 1);
        }
    }

    public static Vector2Int DirectionE
    {
        get
        {
            return new Vector2Int(1, 0);
        }
    }
    public static Vector2Int DirectionW
    {
        get
        {
            return new Vector2Int(-1, 0);
        }
    }

    private const float verticalSpacing = 1.5f;
    private const float horizontalSpacing = 1.73205080757f;
}
