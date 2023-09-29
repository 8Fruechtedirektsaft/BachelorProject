using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell<T> 
{
    internal Cell(Vector2Int mapPosition, T state)
    {
        this.MapPosition = mapPosition;
        this.State = state;
    }

    internal Vector2Int MapPosition;
    internal T State;
}


