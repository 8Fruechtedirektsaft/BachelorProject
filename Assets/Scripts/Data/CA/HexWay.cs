using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexWay 
{
    internal HexWay(List<Vector2Int> fromDirections, Vector2Int toDirection, List<int> authorIDs)
    {
        this.FromDirections = fromDirections;
        this.ToDirection = toDirection;
        this.AuthorIDs = authorIDs;
    }

    internal List<Vector2Int> FromDirections;
    internal Vector2Int ToDirection;
    internal List<int> AuthorIDs;
}
