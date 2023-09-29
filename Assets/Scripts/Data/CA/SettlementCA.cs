using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlementCA : CellularAutomaton<float>
{
    internal SettlementCA(Dictionary<Vector2Int, Cell<float>> initialMapState, Vector2Int mapDimensions) : base(initialMapState, mapDimensions, 2)
    {
        mapKeyList = new(initialMapState.Keys);
    }

    internal override void ApplySteps(int steps)
    {
        for (int step = 0; step < steps; step++)
        {
            Vector2Int cellPositon = CellPositionToUpdate();
            MapState[cellPositon].State = CalculateNewCellState(cellPositon, MapState[cellPositon].State);
            if (MapState[cellPositon].State == 1)
            {
                forestMapLayer.MapState[cellPositon].State = false;
            }
        }
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

    private float CalculateNewCellState(Vector2Int cellPosition, float cellState)
    {
        int height = heightMapLayer.GetCellState(cellPosition);
        if (height == 0 || height == 1 || height == 5)
        {
            return 0;
        }
        int region = regionMapLayer.GetCellState(cellPosition);
        if (region == 0 || region == 4)
        {
            return 0;
        }
        if (riverMapLayer.GetCellState(cellPosition).ToDirection != Vector2Int.zero)
        {
            return 0;
        }
        if (ressourceMapLayer.GetCellState(cellPosition) != 0)
        {
            return 0;
        }

        List<Vector2Int> neighboorPositions = GetNeighboorPositions(cellPosition);
        foreach (Vector2Int neighboorPosition in neighboorPositions)
        {
            if (riverMapLayer.GetCellState(neighboorPosition).ToDirection != Vector2Int.zero)
            {
                cellState += 0.005f;
            }
            if (heightMapLayer.GetCellState(neighboorPosition) == 1)
            {
                cellState += 0.003f;
            }
            if (ressourceMapLayer.GetCellState(neighboorPosition) != 0)
            {
                cellState += 0.05f;
            }
            if (MapState[neighboorPosition].State == 1f)
            {
                return 0;
            }
        }
        cellState = Mathf.Clamp01(cellState);
        return cellState;
    }

    internal void Initialize(CellularAutomaton<int> heightMapLayer, CellularAutomaton<int> regionMapLayer, RiverCA riverMapLayer, RessourceCA ressourceMapLayer, ForestCA forestMapLayer)
    {
        this.heightMapLayer = heightMapLayer;
        this.regionMapLayer = regionMapLayer;
        this.riverMapLayer = riverMapLayer;
        this.ressourceMapLayer = ressourceMapLayer;
        this.forestMapLayer = forestMapLayer;
    }

    private List<Vector2Int> GetNeighboorPositions(Vector2Int cellPosition)
    {
        Vector2Int[] neighboorPositions = HexGridHelper.GetNeighboorPositionsRadiusTwo(cellPosition);
        List<Vector2Int> actualNeighboorPositions = new();
        for (int i = 0; i < neighboorPositions.Length; i++)
        {
            if (MapState.ContainsKey(neighboorPositions[i]))
            {
                actualNeighboorPositions.Add(neighboorPositions[i]);
            }
        }
        return actualNeighboorPositions;
    }

    private List<Vector2Int> mapKeyList;
    private CellularAutomaton<int> heightMapLayer;
    private CellularAutomaton<int> regionMapLayer;
    private RiverCA riverMapLayer;
    private RessourceCA ressourceMapLayer;
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

    internal RessourceCA RessourceMapLayer
    {
        set
        {
            ressourceMapLayer = value;
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
