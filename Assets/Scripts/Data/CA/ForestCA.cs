using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestCA : CellularAutomaton<bool>
{
    internal ForestCA(Dictionary<Vector2Int, Cell<bool>> initialMapState, Vector2Int mapDimensions, int numberToOvercrowd) : base(initialMapState, mapDimensions, 2)
    {
        mapKeyList = new (initialMapState.Keys);
        this.numberToOvercrowd = numberToOvercrowd;
    }

    internal void Initialize(CellularAutomaton<int> heightMapLayer, CellularAutomaton<int> regionMapLayer, RiverCA riverMapLayer)
    {
        this.heightMapLayer = heightMapLayer;
        this.regionMapLayer = regionMapLayer;
        this.riverMapLayer = riverMapLayer;
    }

    internal override void ApplySteps(int steps)
    {
        for (int i = 0; i < steps; i++)
        {
            Vector2Int cellPosition = CellPositionToUpdate();
            UpdateCell(cellPosition);
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

    private void UpdateCell(Vector2Int cellPosition)
    {
        int height = heightMapLayer.GetCellState(cellPosition);
        int region = regionMapLayer.GetCellState(cellPosition);
        if (height == 0 || height == 1 || height == 4 || height == 5)
        {
            MapState[cellPosition].State = false;
            return;
        }
        else if (region == 0 || region == 4)
        {
            MapState[cellPosition].State = false;
            return;
        }
        else if (riverMapLayer.GetCellState(cellPosition).ToDirection != Vector2Int.zero)
        {
            MapState[cellPosition].State = false;
            return;
        }

        if (MapState[cellPosition].State)
        {
            if(NumberAliveNeighboors(cellPosition) > numberToOvercrowd)
            {
                MapState[cellPosition].State = false;
            }
        }

    }

    private int NumberAliveNeighboors(Vector2Int cellPosition)
    {
        Vector2Int[] neighboorPositions = HexGridHelper.GetNeighboorPositionsRadiusTwo(cellPosition);
        int numberAliveNeighboors = 0;
        for (int i = 0; i < neighboorPositions.Length; i++)
        {
            if (MapState.TryGetValue(neighboorPositions[i], out Cell<bool> neighboorCell))
            {
                if (neighboorCell.State)
                {
                    numberAliveNeighboors++;
                }
            }
        }
        return numberAliveNeighboors;
    }

    internal override bool GetCellState(Vector2Int cellPosition)
    {
        return MapState[cellPosition].State;
    }

    private List<Vector2Int> mapKeyList;
    private int numberToOvercrowd;

    private CellularAutomaton<int> heightMapLayer;
    private CellularAutomaton<int> regionMapLayer;
    private RiverCA riverMapLayer;

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
}
