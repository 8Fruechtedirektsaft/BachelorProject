using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TotalisticCA : CellularAutomaton<int>
{
    internal TotalisticCA(Dictionary<Vector2Int, Cell<int>> initialMapState, Vector2Int mapDimensions, int possibleStates, int[,] stateThresholds, float updateChance) : base(initialMapState, mapDimensions, possibleStates)
    {
        this.stateThresholds = stateThresholds;
        this.randomUpdateChance = updateChance;
    }

    internal override void ApplySteps(int steps)
    {
        for (int s = 0; s < steps; s++)
        {
            Dictionary<Vector2Int, Cell<int>> newMapState = new();
            foreach (KeyValuePair<Vector2Int, Cell<int>> entry in MapState)
            {
                Cell<int> newCell = new (entry.Key, CalculateNewCellState(entry.Value));
                newMapState.Add(entry.Key, newCell);
            }
            MapState = newMapState;
        }
    }

    protected List<Cell<int>> GetNeighboorCells(Vector2Int cellPosition)
    {
        Vector2Int[] neighboorPositions = HexGridHelper.GetNeighboorPositions(cellPosition);
        List<Cell<int>> neighboorCells = new();
        for (int i = 0; i < neighboorPositions.Length; i++)
        {
            if (MapState.TryGetValue(neighboorPositions[i], out Cell<int> neighboorCell))
            {
                neighboorCells.Add(neighboorCell);
            }
            else
            {
                neighboorCells.Add(BoundaryCondition(neighboorPositions[i]));
            }
        }
        return neighboorCells; 
    }

    protected Cell<int> BoundaryCondition(Vector2Int cellPosition)
    {
        return new Cell<int>(cellPosition, 0);
    }

    protected int CalculateNewCellState(Cell<int> cell)
    {
        int newState = cell.State;
        if (randomUpdateChance > UnityEngine.Random.value)
        {
            if (UnityEngine.Random.value > 0.5f)
            {
                if (cell.State > 0)
                {
                    newState--;
                }
            }
            else
            {
                if (cell.State < PossibleStates - 1)
                {
                    newState++;
                }
            }
            return newState;
        }

        List<Cell<int>> neighboorCells = GetNeighboorCells(cell.MapPosition);
        int neighboorHoodSum = 0;
        foreach (Cell<int> neighboorCell in neighboorCells)
        {
            neighboorHoodSum += neighboorCell.State;
        }
        if (neighboorHoodSum  >= stateThresholds[cell.State, 1])
        {
            if (newState < PossibleStates - 1)
            {
                newState++;
                return newState;
            }
            else
            {
                newState--;
                return newState;
            }
        }
        if (neighboorHoodSum  >= stateThresholds[cell.State, 0])
        {
            return newState;
        }
        if (newState > 0)
        {
            newState--;
            return newState;
        }
        return 0;
    }

    internal override float CellRiverFrequency(Vector2Int cellPosition)
    {
        if (MapState.TryGetValue(cellPosition, out Cell<int> cell))
        {
            if (cell.State == 2)
            {
                return 0.15f;
            }
            if (cell.State == 3)
            {
                return 0.3f;
            }
            if (cell.State == 4)
            {
                return 0.6f;
            }
            if (cell.State == 5)
            {
                return 1;
            }
        }
        return 0;
    }

    private int[,] stateThresholds; //[state] [0 maintain, 1 ascend]
    private float randomUpdateChance;
}
