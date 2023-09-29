using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveInformation
{ 
    internal ActiveInformation(List<int> authorships, Vector2Int toDirection)
    {
        this.Authorships = authorships;
        this.ToDirection = toDirection;
    }

    internal List<int> Authorships;
    internal Vector2Int ToDirection;
}
