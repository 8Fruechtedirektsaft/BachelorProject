using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RessourceCA : CellularAutomaton<int>
{
    internal RessourceCA(Dictionary<Vector2Int, Cell<int>> initialMapState, Vector2Int mapDimensions, int possibleStates, int[] stateLimits) : base(initialMapState, mapDimensions, possibleStates)
    {
        mapKeyList = new(initialMapState.Keys);
        this.stateLimits = stateLimits;
    }

    internal void Initialize(CellularAutomaton<int> heightMapLayer, CellularAutomaton<int> regionMapLayer, RiverCA riverMapLayer, ForestCA forestMapLayer)
    {
        this.heightMapLayer = heightMapLayer;
        this.regionMapLayer = regionMapLayer;
        this.riverMapLayer = riverMapLayer;
        this.forestMapLayer = forestMapLayer;
    }

    internal override void ApplySteps(int steps)
    {
        for (int step = 0; step < steps; step++)
        {
            Vector2Int cellPositon = CellPositionToUpdate();
            UpdateCell(cellPositon);          
        }
    }

    internal bool TryRemoveRessource(Vector2Int ressourcePosition)
    {
        if (MapState.TryGetValue(ressourcePosition, out Cell<int> ressource))
        {
            if (ressource.State != 0)
            {
                stateLimits[ressource.State - 1]++;
                MapState[ressourcePosition].State = 0;
                return true;
            }
        }
        return false;
    }

    private void UpdateCell(Vector2Int cellPosition)
    {
        for (int state = 1; state <= PossibleStates; state++)
        {
            if (stateLimits[state - 1] > 0 && IsValidPositionForState(state, cellPosition))
            {
                MapState[cellPosition].State = state;
                stateLimits[state - 1]--;
            }
        }
    }

    private bool IsValidPositionForState(int cellState, Vector2Int cellPosition)
    {
        if (MapState[cellPosition].State != 0)
        {
            return false;
        }
        LayerRules rules = statesLayerRules[cellState-1];
        if (!rules.AllowedHeight.Contains(heightMapLayer.GetCellState(cellPosition)))
        {
            return false;
        }
        if (!rules.AllowedRegion.Contains(regionMapLayer.GetCellState(cellPosition)))
        {
            return false;
        }
        if (!((riverMapLayer.GetCellState(cellPosition).ToDirection != Vector2Int.zero) == rules.RiverPresent))
        {
            return false;
        }
        if (!(forestMapLayer.GetCellState(cellPosition) == rules.ForestPresent))
        {
            return false;
        }
        int numberSameStateNeighboors = NumberSameStateNeighboors(cellState, cellPosition);
        if (numberSameStateNeighboors > rules.ToleratedEqualStates)
        {
            return false;
        }

        return true;
    }

    private int NumberSameStateNeighboors(int cellState, Vector2Int cellPosition)
    {
        Vector2Int[] neighboorPositions = HexGridHelper.GetNeighboorPositionsRadiusTwo(cellPosition);
        int numberSameStateNeighboors = 0;
        for (int i = 0; i < neighboorPositions.Length; i++)
        {
            if (MapState.TryGetValue(neighboorPositions[i], out Cell<int> neighboorCell))
            {
                if (neighboorCell.State == cellState)
                {
                    numberSameStateNeighboors++;
                }
            }
        }
        return numberSameStateNeighboors;
    }

    private Vector2Int CellPositionToUpdate()
    {
        if (mapKeyList.Count == 0)
        {
            mapKeyList = new(MapState.Keys);
        }
        int randomIndex = UnityEngine.Random.Range(0, mapKeyList.Count);
        Vector2Int cellPosition = mapKeyList[randomIndex];
        mapKeyList.RemoveAt(randomIndex);
        return cellPosition;
    }

    private LayerRules[] statesLayerRules =
    {
        new(new int[]{2, 3}, new int[]{2, 3}, true, false, 1),
        new(new int[]{3, 4}, new int[]{0, 1, 2, 3, 4}, false, false, 0),
        new(new int[]{2, 3, 4}, new int[]{3}, false, true, 1),
    };
    private List<Vector2Int> mapKeyList;
    private int[] stateLimits;

    private CellularAutomaton<int> heightMapLayer;
    private CellularAutomaton<int> regionMapLayer;
    private RiverCA riverMapLayer;
    private ForestCA forestMapLayer;

    internal CellularAutomaton<int> HeightMapLayer
    {
        set
        {
            heightMapLayer = value;
        }
    }
    internal CellularAutomaton<int> RegionMapLayer
    {
        set
        {
            regionMapLayer = value;
        }
    }
    internal RiverCA RiverMapLayer
    {
        set
        {
            riverMapLayer = value;
        }
    }

    internal ForestCA ForestMapLayer
    {
        set
        {
            forestMapLayer = value;
        }
    }
}
