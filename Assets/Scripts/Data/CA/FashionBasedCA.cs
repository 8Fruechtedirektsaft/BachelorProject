using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FashionBasedCA : CellularAutomaton<int>
{
    internal FashionBasedCA(Dictionary<Vector2Int, Cell<int>> initialMapState, Vector2Int mapDimensions, int possibleStates, float[,] evaluationTable, float scoreBonus) : base(initialMapState, mapDimensions, possibleStates)
    {
        this.evaluationTable = evaluationTable;
        scoreMap = new();
        scoreLatitudeBouns = scoreBonus;
    }

    protected int CalculateNewCellState(Cell<int> cell)
    {
        List<Cell<int>> neighboorCells = GetNeighboorCells(cell.MapPosition);
        float highScore = 0;
        int newCellState = cell.State;
        foreach (Cell<int> neighboorCell in neighboorCells)
        {
            float neighboorScore = scoreMap[neighboorCell.MapPosition];
            if (neighboorScore > highScore)
            {
                highScore = neighboorScore;
                newCellState = neighboorCell.State;
            }
            else if (neighboorScore == highScore)
            {
                int difOld = Mathf.Abs(newCellState - cell.State);
                int difNew = Mathf.Abs(neighboorCell.State - cell.State);
                if (difOld < difNew)
                {
                    highScore = neighboorScore;
                    newCellState = neighboorCell.State;
                }
            }
        }
        if(highScore > scoreMap[cell.MapPosition])
        {
            return newCellState;
        }
        return cell.State;
    }

    private float CalculateScore(Cell<int> cell)
    {
        List<int> neighboorStates = GetNeighboorStates(cell.MapPosition);
        float score = 0;
        foreach (int neighboorState in neighboorStates)
        {
            score += evaluationTable[cell.State, neighboorState];
        }
        score += CalculateScoreBonus(cell);
        return score;
    }

    private float CalculateScoreBonus(Cell<int> cell)
    {
        float latitude = 0;
        if(cell.MapPosition.y != 0)
        {
            latitude = (float)Mathf.Abs(cell.MapPosition.y) / (float)MapDimensions.y;
        }
        if(latitude >= scoreLatitudeBonusRange[cell.State][0] && latitude <= scoreLatitudeBonusRange[cell.State][1])
        {
            return scoreLatitudeBouns;
        }
        else if(latitude >= 1-scoreLatitudeBonusRange[cell.State][1] && latitude <= 1 - scoreLatitudeBonusRange[cell.State][0])
        {
            return scoreLatitudeBouns;
        }
        return 0;
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
        }
        return neighboorCells;
    }

    private List<int> GetNeighboorStates(Vector2Int cellPosition)
    {
        Vector2Int[] neighboorPositions = HexGridHelper.GetNeighboorPositions(cellPosition);
        List<int> neighboorStates = new();
        for (int i = 0; i < neighboorPositions.Length; i++)
        {
            if (MapState.TryGetValue(neighboorPositions[i], out Cell<int> neighboorCell))
            {
                neighboorStates.Add(neighboorCell.State);
            }
        }
        return neighboorStates;
    }

    internal override void ApplySteps(int steps)
    {
        Debug.Log("start");
        for (int s = 0; s < steps; s++)
        {
            scoreMap.Clear();
            foreach (KeyValuePair<Vector2Int, Cell<int>> entry in MapState)
            {
                float cellScore = CalculateScore(entry.Value);
                scoreMap.Add(entry.Key, cellScore);
            }
            Dictionary<Vector2Int, Cell<int>> newMapState = new();
            foreach (KeyValuePair<Vector2Int, Cell<int>> entry in MapState)
            {
                Cell<int> newCell = new(entry.Key, CalculateNewCellState(entry.Value));
                newMapState.Add(entry.Key, newCell);
            }
            MapState = newMapState;
        }
        Debug.Log("ende");
    }

    internal override float CellRiverFrequency(Vector2Int cellPosition)
    {
        if(MapState.TryGetValue(cellPosition, out Cell<int> cell))
        {
            if (cell.State == 1)
            {
                return 0.1f;
            }
            if (cell.State == 2)
            {
                return 0.15f;
            }
            if (cell.State == 3)
            {
                return 0.25f;
            }
            if (cell.State == 4)
            {
                return 0.01f;
            }
        }
        return 0;
    }

    private readonly float[,] evaluationTable;
    private Dictionary<Vector2Int, float> scoreMap;
    private float scoreLatitudeBouns;
    private float[][] scoreLatitudeBonusRange = new float[][]
    {
    new float[] {0, 0.075f}, //polar
    new float[] {0.05f, 0.2f}, //tundra
    new float[] {0.15f, 0.35f}, //temperate
    new float[] {0.3f, 0.5f}, //tropical
    new float[] {0.425f, 0.5f} //desert
    };

    internal float[][] ScoreBonusRange
    {
        set
        {
            scoreLatitudeBonusRange = value;
        }
    }

}
