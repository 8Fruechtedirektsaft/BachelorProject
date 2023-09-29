using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerRules 
{
    internal LayerRules(int[] height, int[] region, bool riverPresent, bool forestPresent, int toleratedEqualStates)
    {
        this.AllowedHeight = height;
        this.AllowedRegion = region;
        this.RiverPresent = riverPresent;
        this.ForestPresent = forestPresent;
        this.ToleratedEqualStates = toleratedEqualStates;
    }

    internal readonly int[] AllowedHeight;
    internal readonly int[] AllowedRegion;
    internal readonly bool RiverPresent;
    internal readonly bool ForestPresent;
    internal readonly int ToleratedEqualStates;
}
