using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Profiling;

public class HexMapData : MonoBehaviour
{
    public void Start()
    {
        UnityEngine.Random.InitState(seed);

        Dictionary<Vector2Int, Cell<int>> regionInitialState = BaseMapGenerator.GenerateRandomCellMap(mapWidth, mapHeight, regionPossibleStates);
        FashionBasedCA regionMapCA = new(regionInitialState, new(mapWidth, mapHeight), regionPossibleStates, GetFashionMatrix(), fashionScoreBonus);

        Dictionary<Vector2Int, Cell<int>> heightInitialState = BaseMapGenerator.GenerateRandomCellMap(mapWidth, mapHeight, heightPossibleStates, heightEmptyPercentage);
        TotalisticCA heightMapCA = new(heightInitialState, new(mapWidth, mapHeight), heightPossibleStates, GetHeightMatrix(), heightChance);

        Dictionary<Vector2Int, Cell<HexWay>> riverInitialState = BaseMapGenerator.GenerateEmptyCellHexwayMap(mapWidth, mapHeight);
        RiverCA riverMapCA = new(riverInitialState, new(mapWidth, mapHeight), 0, riverChance);

        Dictionary<Vector2Int, Cell<bool>> forestInitialState = BaseMapGenerator.GenerateFullCellBoolMap(mapWidth, mapHeight);
        ForestCA forestMapCA = new(forestInitialState, new(mapWidth, mapHeight), forestOverCrowd);

        Dictionary<Vector2Int, Cell<int>> ressourceInitialState = BaseMapGenerator.GenerateEmptyCellMap(mapWidth, mapHeight);
        RessourceCA ressourceMapCA = new(ressourceInitialState, new(mapWidth, mapHeight), 3, ressourceStatesAmounts);

        Dictionary<Vector2Int, Cell<float>> settlementInitialState = BaseMapGenerator.GenerateEmptyCellFloatMap(mapWidth, mapHeight);
        SettlementCA settlementMapCA = new(settlementInitialState, new(mapWidth, mapHeight));

        terrain = new(regionMapCA, heightMapCA, riverMapCA, forestMapCA, ressourceMapCA, settlementMapCA);
        terrain.RegionMapUpdatedEvent += OnRegionMapUpdatedEvent;
        terrain.HeightMapUpdatedEvent += OnHeightMapUpdatedEvent;
        terrain.RiverMapUpdatedEvent += OnRiverMapUpdatedEvent;
        terrain.ForestMapUpdatedEvent += OnForestMapUpdatedEvent;
        terrain.RessourceMapUpdatedEvent += OnRessourceMapUpdatedEvent;
        terrain.SettlementMapUpdatedEvent += OnSettlementUpdatedEvent;
    }

    private float[,] GetFashionMatrix()
    {
        float[,] fashionMatrix = new float[regionPossibleStates, regionPossibleStates];
        int l = 0;
        for (int j = 0; j < regionPossibleStates; j++)
        {
            for (int i = 0; i < regionPossibleStates; i++)
            {
                fashionMatrix[i, j] = fashionMatrixValues[l];
                l++;
            }
        }
        return fashionMatrix;
    }

    private int[,] GetHeightMatrix()
    {
        int[,] heightMatrix = new int[heightPossibleStates, 2];
        int l = 0;
        for (int i = 0; i < heightPossibleStates; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                heightMatrix[i, j] = heightStateThresholds[l];
                l++;
            }
        }
        return heightMatrix;
    }

    internal void UpdateRegionMap(int updateSteps)
    {
        terrain.UpdateRegionMap(updateSteps);
    }

    internal void UpdateHeightMap(int updateSteps)
    {
        terrain.UpdateHeightMap(updateSteps);
    }

    internal void UpdateRiverMap(int updateSteps)
    {
        terrain.UpdateRiverMap(updateSteps);
    }

    internal void UpdateForestMap(int updateSteps)
    {
        terrain.UpdateForestMap(updateSteps);
    }

    internal void UpdateRessourceMap(int updateSteps)
    {
        terrain.UpdateRessourceMap(updateSteps);
    }

    internal bool TryRemoveRessource(Vector2Int ressourcePosition)
    {
        return terrain.TryRemoveRessource(ressourcePosition);
    }

    internal void UpdateSettlementMap(int updateSteps)
    {
        terrain.UpdateSettlementMap(updateSteps);
    }

    internal void UpdateHeightMapOnly(int t)
    {
        terrain.UpdateHeightMap(t);
        dataViewAdapter.GenerateHeightMapView();
    }

    internal void UpdateRegionMapOnly(int updateSteps)
    {
        terrain.UpdateRegionMap(updateSteps);
        dataViewAdapter.GenerateRegionMapView();
    }

    internal void ClearOnlyMaps()
    {
        dataViewAdapter.ClearExtraMaps();
    }

    private void OnRessourceMapUpdatedEvent()
    {
        dataViewAdapter.GenerateRessourceMapView();
    }

    private void OnRegionMapUpdatedEvent()
    {
        dataViewAdapter.GenerateTileMapView();
    }

    private void OnHeightMapUpdatedEvent()
    {
        dataViewAdapter.GenerateTileMapView();
    }

    private void OnRiverMapUpdatedEvent()
    {
        dataViewAdapter.GenerateRiverMapView();
    }

    private void OnForestMapUpdatedEvent()
    {
        dataViewAdapter.GenerateForestMapView();
    }

    private void OnSettlementUpdatedEvent()
    {
        dataViewAdapter.GenerateSettlementMapView();
    }

    [Header("Map Options")]
    [SerializeField]
    private int seed;
    [SerializeField]
    private int mapWidth = 10;
    [SerializeField]
    private int mapHeight = 10;
    [SerializeField]
    private int[] ressourceStatesAmounts;
    [SerializeField]
    private int forestOverCrowd;
    [SerializeField]
    private float riverChance;
    [SerializeField]
    private int heightPossibleStates;
    [SerializeField]
    private float heightChance;
    [SerializeField]
    private float heightEmptyPercentage;
    [SerializeField]
    private int[] heightStateThresholds;
    [SerializeField]
    private int regionPossibleStates;
    [SerializeField]
    private float[] fashionMatrixValues;
    [SerializeField]
    private float fashionScoreBonus;
    [Header("Other")]
    [SerializeField]
    private DataViewAdapter dataViewAdapter;

    private HexTerrain terrain;

    internal Dictionary<Vector2Int, int> MovementCostMap
    {
        get
        {
            return terrain.MovementCostMap;
        }
    }

    internal Dictionary<Vector2Int, int> RessourceMap
    {
        get
        {
            return terrain.RessourceMapStates;
        }
    }

    internal Dictionary<Vector2Int, int> RegionMap
    {
        get
        {
            return terrain.RegionMapStates;
        }
    }

    internal Dictionary<Vector2Int, int> HeightMap
    {
        get
        {
            return terrain.HeightMapStates;
        }
    }

    internal Dictionary<Vector2Int, HexWay> RiverMap
    {
        get
        {
            return terrain.RiverMapStates;
        }
    }

    internal List<Vector2Int> RiverSpringPositions
    {
        get
        {
            return terrain.RiverSpringPositions;
        }
    }

    internal Dictionary<Vector2Int, bool> ForestMap
    { 
        get
        {
            return terrain.ForestMapStates;
        }
    }

    internal Dictionary<Vector2Int, bool> SettlementMap
    {
        get
        {
            return terrain.SettlementMapStates;
        }
    }

    internal  Vector2Int MapDimensions
    {
        get
        {
            return new(terrain.MapWidth, terrain.MapHeight);
        }
    }
}
