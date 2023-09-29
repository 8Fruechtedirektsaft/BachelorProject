using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public static class BaseMapGenerator
{
    private static int GetStateFromChance(int possibleStates, float chance)
    {
        chance = Mathf.Clamp01(chance);
        for (int state = possibleStates-1; state > 0; state--)
        {
            if (chance >= (float)state / (float)possibleStates)
            {
                return state;
            }
        }
        return 0;
    }

    internal static Dictionary<Vector2Int, Cell<int>> GenerateRandomCellMap(int width, int height, int possibleCellStates)
    {
        Dictionary<Vector2Int, Cell<int>> cellMap = new();
        for (int y = 0; y < height; y++)
        {
            int offset = y / 2;
            for (int x = -offset; x < width - offset; x++)
            {
                Vector2Int mapPosition = new(x, y);
                Cell<int> newCell = new(mapPosition, GetStateFromChance(possibleCellStates, UnityEngine.Random.value));
                cellMap.Add(mapPosition, newCell);
            }
        }
        return cellMap;
    }

    internal static Dictionary<Vector2Int, Cell<int>> GenerateRandomCellMap(int width, int height, int possibleCellStates, float emptyChance)
    {
        Dictionary<Vector2Int, Cell<int>> cellMap = new();
        for (int y = 0; y < height; y++)
        {
            int offset = y / 2;
            for (int x = -offset; x < width - offset; x++)
            {
                Vector2Int mapPosition = new(x, y);
                int newCellState = 0;
                if (UnityEngine.Random.value > emptyChance)
                {
                    newCellState = GetStateFromChance(possibleCellStates - 1, UnityEngine.Random.value) + 1;
                }
                Cell<int> newCell = new(mapPosition, newCellState);
                cellMap.Add(mapPosition, newCell);
            }
        }
        return cellMap;
    }

    internal static Dictionary<Vector2Int, Cell<int>> GenerateEmptyCellMap(int width, int height)
    {
        Dictionary<Vector2Int, Cell<int>> cellMap = new();
        for (int y = 0; y < height; y++)
        {
            int offset = y / 2;
            for (int x = -offset; x < width - offset; x++)
            {
                Vector2Int mapPosition = new(x, y);
                Cell<int> newCell = new(mapPosition, 0);
                cellMap.Add(mapPosition, newCell);
            }
        }
        return cellMap;
    }

    internal static Dictionary<Vector2Int, Cell<HexWay>> GenerateEmptyCellHexwayMap(int width, int height)
    {
        Dictionary<Vector2Int, Cell<HexWay>> cellMap = new();
        for (int y = 0; y < height; y++)
        {
            int offset = y / 2;
            for (int x = -offset; x < width - offset; x++)
            {
                Vector2Int mapPosition = new(x, y);
                Cell<HexWay> newCell = new(mapPosition, new(new(), Vector2Int.zero, new()));
                cellMap.Add(mapPosition, newCell);
            }
        }
        return cellMap;
    }

    internal static Dictionary<Vector2Int, Cell<float>> GenerateEmptyCellFloatMap(int width, int height)
    {
        Dictionary<Vector2Int, Cell<float>> cellMap = new();
        for (int y = 0; y < height; y++)
        {
            int offset = y / 2;
            for (int x = -offset; x < width - offset; x++)
            {
                Vector2Int mapPosition = new(x, y);
                Cell<float> newCell = new(mapPosition, 0);
                cellMap.Add(mapPosition, newCell);
            }
        }
        return cellMap;
    }

    internal static Dictionary<Vector2Int, Cell<bool>> GenerateFullCellBoolMap(int width, int height)
    {
        Dictionary<Vector2Int, Cell<bool>> cellMap = new();
        for (int y = 0; y < height; y++)
        {
            int offset = y / 2;
            for (int x = -offset; x < width - offset; x++)
            {
                Vector2Int mapPosition = new(x, y);
                Cell<bool> newCell = new(mapPosition, true);
                cellMap.Add(mapPosition, newCell);
            }
        }
        return cellMap;
    }
}
