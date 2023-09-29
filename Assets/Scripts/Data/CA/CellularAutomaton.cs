using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CellularAutomaton<T>
{
    protected CellularAutomaton(Dictionary<Vector2Int, Cell<T>> initialMapState, Vector2Int mapDimensions, int possibleStates)
    {
        MapState = initialMapState;
        this.MapDimensions = mapDimensions;
        this.PossibleStates = possibleStates;
    }

    internal abstract void ApplySteps(int steps);

    internal virtual float CellRiverFrequency(Vector2Int cellPosition)
    {
        return 0;
    }

    internal readonly int PossibleStates;

    internal readonly Vector2Int MapDimensions;
    
    internal Dictionary<Vector2Int, Cell<T>> MapState;

    internal virtual T GetCellState(Vector2Int cellPosition)
    {
        return MapState[cellPosition].State;
    }
}
