using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class RiverCA : CellularAutomaton<HexWay>
{
    internal RiverCA(Dictionary<Vector2Int, Cell<HexWay>> initialMapState, Vector2Int mapDimensions, int possibleStates, float globalRiverChance) : base(initialMapState, mapDimensions, possibleStates)
    {
        this.globalRiverChance = globalRiverChance;
    }

    internal void Initialize(CellularAutomaton<int> heightMapLayer, CellularAutomaton<int> regionMapLayer)
    {
        this.heightMapLayer = heightMapLayer;
        this.regionMapLayer = regionMapLayer;
        DistributeActiveCells();
    }

    private void DistributeActiveCells()
    {
        SpringPositions = new();
        activeRemovingCells = new();
        currentActiveCells = new();
        nextActiveCells = new();
        int authorID = 0;
        foreach (KeyValuePair <Vector2Int, Cell<HexWay>> entry in MapState)
        {
            float springChance = GetSpringChance(entry.Key);
            if (springChance > UnityEngine.Random.value)
            {
                Vector2Int[] directions = HexGridHelper.GetNeighboorPositions(Vector2Int.zero);
                bool foundDirection = false;
                Vector2Int toDirection = Vector2Int.zero;
                while (!foundDirection)
                {
                    int directionIndex = UnityEngine.Random.Range(0, directions.Length);
                    toDirection = directions[directionIndex];
                    if (MapState.ContainsKey(entry.Key + toDirection))
                    {
                        foundDirection = true;
                    }
                }
                currentActiveCells.Add(new(entry.Key, new(new() { authorID }, toDirection)));
                SpringPositions.Add(entry.Key);
                authorID++;
            }
        }
    }

    private float GetSpringChance(Vector2Int mapPosition)
    {
        Vector2Int[] neighboorPositions = HexGridHelper.GetNeighboorPositions(mapPosition);
        Vector2Int[] positions = new Vector2Int[neighboorPositions.Length + 1];
        for (int j = 0; j < neighboorPositions.Length; j++)
        {
            for (int k = 0; k < currentActiveCells.Count; k++)
            {
                if (currentActiveCells[k].MapPosition == neighboorPositions[j])
                {
                    return 0;
                }
            }
            positions[j] = neighboorPositions[j];
        }
        positions[neighboorPositions.Length] = mapPosition;
        if (heightMapLayer.GetCellState(positions[^1]) == heightMapLayer.PossibleStates - 1)
        {
            return 0;
        }

        float heightChance = 0;
        float regionChance = 0;
        for (int i = 0; i < positions.Length; i++)
        {
            float neighboorHeightChance = heightMapLayer.CellRiverFrequency(positions[i]);
            float neighboorRegionChance = regionMapLayer.CellRiverFrequency(positions[i]);
            if (neighboorHeightChance <= 0 || neighboorRegionChance <= 0)
            {
                return 0;
            }
            heightChance += neighboorHeightChance;
            regionChance += neighboorRegionChance;
        }
        float springChance = (heightChance + regionChance) * globalRiverChance;
        return springChance;
    }

    internal override void ApplySteps(int steps)
    {
        for (int step = 0; step < steps; step++)
        {
            for (int i = 0; i < currentActiveCells.Count;)
            {
                Cell<ActiveInformation> activeCell = currentActiveCells[i];
                currentActiveCells.RemoveAt(i);
                UpdateActiveCell(activeCell);
            }
            currentActiveCells = nextActiveCells;
            nextActiveCells = new();
        }
    }

    internal void UpdateUntilStable()
    {
        int count = 0;
        while (currentActiveCells.Count > 0)
        {
            for (int i = 0; i < currentActiveCells.Count;)
            {
                Cell<ActiveInformation> activeCell = currentActiveCells[i];
                currentActiveCells.RemoveAt(i);
                UpdateActiveCell(activeCell);
            }
            currentActiveCells = nextActiveCells;
            nextActiveCells = new();
            count++;
        }
        Debug.Log(count);
    }

    private void UpdateActiveRemovingCell(Cell<Vector2Int> removingCell)
    {
        if (MapState.TryGetValue(removingCell.MapPosition, out Cell<HexWay> foundCell))
        {
            for (int j = 0; j < currentActiveCells.Count; j++)
            {
                if (currentActiveCells[j].MapPosition == removingCell.MapPosition)
                {
                    currentActiveCells.RemoveAt(j);
                }
            }
            for (int k = 0; k < nextActiveCells.Count; k++)
            {
                if (nextActiveCells[k].MapPosition == removingCell.MapPosition)
                {
                    nextActiveCells.RemoveAt(k);
                }
            }
            if (foundCell.State.ToDirection != Vector2Int.zero)
            {
                activeRemovingCells.Add(new(removingCell.MapPosition + foundCell.State.ToDirection, HexGridHelper.GetOppositeDirection(foundCell.State.ToDirection)));
            }
            if (foundCell.State.FromDirections.Count > 1)
            {
                foreach (Vector2Int fromDirection in foundCell.State.FromDirections)
                {
                    if (fromDirection != removingCell.State)
                    {
                        Vector2Int cutOffRiverPosition = removingCell.MapPosition + fromDirection;
                        Cell<HexWay> cutOffCell = new(cutOffRiverPosition, new(new(), Vector2Int.zero, new()));
                        currentActiveCells.Add(new(cutOffRiverPosition, new(MapState[cutOffRiverPosition].State.AuthorIDs, HexGridHelper.GetOppositeDirection(MapState[cutOffRiverPosition].State.FromDirections[0]))));
                        MapState[cutOffRiverPosition] = cutOffCell;
                    }
                }
            }
            Cell<HexWay> newCell = new(removingCell.MapPosition, new(new(), Vector2Int.zero, new()));
            MapState[removingCell.MapPosition] = newCell;
        }
    }

    private void UpdateActiveCell(Cell<ActiveInformation> activeCell)
    {
        MapState[activeCell.MapPosition].State.FromDirections.Add(HexGridHelper.GetOppositeDirection(activeCell.State.ToDirection));
        MapState[activeCell.MapPosition].State.AuthorIDs.AddRange(activeCell.State.Authorships);

        List<Vector2Int> nextDirections = HexGridHelper.GetGeneralDirections(activeCell.State.ToDirection);
        SetFlowDirection(activeCell.MapPosition, activeCell.State.Authorships, nextDirections);
    }

    private void SetFlowDirection(Vector2Int cellPosition, List<int> activeAuthorships, List<Vector2Int> possibleDirections)
    {
        for (int i = 0; i < possibleDirections.Count; i++)
        {
            if (!MapState.ContainsKey(cellPosition + possibleDirections[i]))
            {
                //cut border
                possibleDirections.RemoveAt(i);
                i--;
            }
            else if (heightMapLayer.GetCellState(cellPosition + possibleDirections[i]) > heightMapLayer.GetCellState(cellPosition))
            {
                possibleDirections.RemoveAt(i);
                i--;
            }
            else if (SpringPositions.Contains(cellPosition + possibleDirections[i]))
            {
                possibleDirections.RemoveAt(i);
                i--;
            }
        }

        bool oceanFound = false;
        List<Vector2Int> oceanDirections = new();
        for (int i = 0; i < possibleDirections.Count; i++)
        {
            if (heightMapLayer.GetCellState(possibleDirections[i] + cellPosition) <= 1)
            {
                //ocean found
                oceanFound = true;
                oceanDirections.Add(possibleDirections[i]);
            }
        }
        if (oceanFound)
        {
            Vector2Int randomDirection = RandomFromList(oceanDirections);
            MapState[cellPosition].State.ToDirection = randomDirection;
            return;
        }


        for (int i = 0; i < possibleDirections.Count; i++)
        {
            Vector2Int nextCellPosition = possibleDirections[i] + cellPosition;
            for (int j = 0; j < currentActiveCells.Count; j++)
            {
                if (currentActiveCells[j].MapPosition == nextCellPosition)
                {
                    Vector2Int fromDirectionCellCurrent = HexGridHelper.GetOppositeDirection(currentActiveCells[j].State.ToDirection);
                    Vector2Int fromDirectionCellNew = HexGridHelper.GetOppositeDirection(possibleDirections[i]);
                    List<int> combinedAuthorships = activeAuthorships;
                    combinedAuthorships.AddRange(currentActiveCells[j].State.Authorships);
                    currentActiveCells.RemoveAt(j);
                    MapState[cellPosition].State.ToDirection = possibleDirections[i];
                    MapState[nextCellPosition].State.FromDirections.Add(HexGridHelper.GetOppositeDirection(possibleDirections[i]));
                    MapState[nextCellPosition].State.FromDirections.Add(fromDirectionCellCurrent);
                    MapState[nextCellPosition].State.AuthorIDs.AddRange(combinedAuthorships);
                    List<Vector2Int> collisionDirections = GetDirectionsFromCollision(fromDirectionCellCurrent, fromDirectionCellNew);
                    SetFlowDirection(nextCellPosition, combinedAuthorships, collisionDirections);
                    return;
                }
            }
            for (int j = 0; j < nextActiveCells.Count; j++)
            {
                if (nextActiveCells[j].MapPosition == nextCellPosition)
                {
                    Vector2Int fromDirectionCellCurrent = HexGridHelper.GetOppositeDirection(nextActiveCells[j].State.ToDirection);
                    Vector2Int fromDirectionCellNew = HexGridHelper.GetOppositeDirection(possibleDirections[i]);
                    List<int> combinedAuthorships = activeAuthorships;
                    combinedAuthorships.AddRange(nextActiveCells[j].State.Authorships);
                    nextActiveCells.RemoveAt(j);
                    MapState[cellPosition].State.ToDirection = possibleDirections[i];
                    MapState[nextCellPosition].State.FromDirections.Add(HexGridHelper.GetOppositeDirection(possibleDirections[i]));
                    MapState[nextCellPosition].State.FromDirections.Add(fromDirectionCellCurrent);
                    MapState[nextCellPosition].State.AuthorIDs.AddRange(combinedAuthorships);
                    List<Vector2Int> collisionDirections = GetDirectionsFromCollision(fromDirectionCellCurrent, fromDirectionCellNew);
                    SetFlowDirection(nextCellPosition, combinedAuthorships, collisionDirections);
                    return;
                }
            }
            if (MapState[nextCellPosition].State.FromDirections.Count == 1)
            {
                if (MapState[nextCellPosition].State.AuthorIDs.Any(riverID => activeAuthorships.Any(activeID => activeID == riverID)))
                {
                    //dont flow into itself
                    possibleDirections.RemoveAt(i);
                    i--;
                }
                else
                {
                    //delete downstream
                    if (MapState[nextCellPosition].State.ToDirection != Vector2Int.zero)
                    {
                        activeRemovingCells.Add(new(MapState[nextCellPosition].State.ToDirection + MapState[nextCellPosition].MapPosition, HexGridHelper.GetOppositeDirection(MapState[nextCellPosition].State.ToDirection)));
                        while (activeRemovingCells.Count > 0)
                        {
                            UpdateActiveRemovingCell(activeRemovingCells[0]);
                            activeRemovingCells.RemoveAt(0);
                        }
                    }

                    Vector2Int fromDirectionCellCurrent = MapState[nextCellPosition].State.FromDirections[0];
                    Vector2Int fromDirectionCellNew = HexGridHelper.GetOppositeDirection(possibleDirections[i]);
                    List<int> combinedAuthorships = activeAuthorships;
                    combinedAuthorships.AddRange(MapState[nextCellPosition].State.AuthorIDs);
                    MapState[cellPosition].State.ToDirection = possibleDirections[i];
                    MapState[nextCellPosition].State.FromDirections.Add(HexGridHelper.GetOppositeDirection(possibleDirections[i]));
                    MapState[nextCellPosition].State.AuthorIDs.AddRange(combinedAuthorships);
                    List<Vector2Int> collisionDirections = GetDirectionsFromCollision(fromDirectionCellCurrent, fromDirectionCellNew);
                    SetFlowDirection(nextCellPosition, combinedAuthorships, collisionDirections);
                    return;
                }
            }
        }


        if (possibleDirections.Count > 0)
        {
            Vector2Int possibleCellPosition = possibleDirections[0] + cellPosition;
            float lowestHeight = heightMapLayer.GetCellState(possibleCellPosition);
            for (int i = 1; i < possibleDirections.Count; i++)
            {
                possibleCellPosition = possibleDirections[i] + cellPosition;
                int heightInDirection = heightMapLayer.GetCellState(possibleCellPosition);
                if (heightInDirection < lowestHeight)
                {
                    //lower height found
                    for (int j = i - 1; j >= 0; j--)
                    {
                        possibleDirections.RemoveAt(j);
                    }
                    lowestHeight = heightInDirection;
                    i = 0;
                }
                else if (heightInDirection < lowestHeight)
                {
                    possibleDirections.RemoveAt(i);
                    i--;
                }
            }
        }

        if (possibleDirections.Count > 0)
        {
            Vector2Int possibleCellPosition = possibleDirections[0] + cellPosition;
            float bestRegionValue = regionMapLayer.CellRiverFrequency(possibleCellPosition);
            for (int i = 1; i < possibleDirections.Count; i++)
            {
                possibleCellPosition = possibleDirections[i] + cellPosition;
                float possibleCellRegionValue = regionMapLayer.CellRiverFrequency(possibleCellPosition);
                if (possibleCellRegionValue > bestRegionValue)
                {
                    //better region found
                    for (int j = i - 1; j >= 0; j--)
                    {
                        possibleDirections.RemoveAt(j);
                    }
                    bestRegionValue = possibleCellRegionValue;
                    i = 0;
                }
                else if (possibleCellRegionValue < bestRegionValue)
                {
                    possibleDirections.RemoveAt(i);
                    i--;
                }
            }
        }

        if (possibleDirections.Count > 0)
        {
            Vector2Int randomDirection = RandomFromList(possibleDirections);
            MapState[cellPosition].State.ToDirection = randomDirection;
            nextActiveCells.Add(new(cellPosition + randomDirection, new(activeAuthorships, randomDirection)));
        }
        else
        {
            heightMapLayer.MapState[cellPosition].State = 1;
            MapState[cellPosition].State.FromDirections = new();
            SpringPositions.Remove(cellPosition);
        }
    }

    private Vector2Int RandomFromList(List<Vector2Int> directions)
    {
        int randomIndex = UnityEngine.Random.Range(0, directions.Count);
        return directions[randomIndex];
    }

    private List<Vector2Int> GetDirectionsFromCollision(Vector2Int firstDirection, Vector2Int secondDirection)
    {
        List<Vector2Int> newDirections = new();
        int clockRotations = HexGridHelper.GetClockRotationsBetween(firstDirection, secondDirection);
        if (clockRotations == 3)
        {
            newDirections.Add(HexGridHelper.GetNeighboorClockwise(firstDirection));
            newDirections.Add(HexGridHelper.GetNeighboorCounterClockwise(firstDirection));
            newDirections.Add(HexGridHelper.GetNeighboorClockwise(secondDirection));
            newDirections.Add(HexGridHelper.GetNeighboorCounterClockwise(secondDirection));
            return newDirections;
        }
        if (clockRotations == 2)
        {
            newDirections.Add(HexGridHelper.GetNeighboorCounterClockwise(firstDirection, 2));
            return newDirections;
        }
        if (clockRotations == 4)
        {
            newDirections.Add(HexGridHelper.GetNeighboorClockwise(firstDirection, 2));
            return newDirections;
        }
        if (clockRotations == 1 || clockRotations == 5)
        {
            newDirections.Add(HexGridHelper.GetOppositeDirection(firstDirection));
            newDirections.Add(HexGridHelper.GetOppositeDirection(secondDirection));
            return newDirections;

        }
        return newDirections;
    }

    private float globalRiverChance;
    private List<Cell<ActiveInformation>> currentActiveCells;
    private List<Cell<ActiveInformation>> nextActiveCells;
    private List<Cell<Vector2Int>> activeRemovingCells;
    private CellularAutomaton<int> heightMapLayer;
    private CellularAutomaton<int> regionMapLayer;
    internal List<Vector2Int> SpringPositions;
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

    internal bool Initialized = false;
}

