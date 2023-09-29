using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HexTerrain
{
    internal HexTerrain(CellularAutomaton<int> regionMapCA, CellularAutomaton<int> heightMapCA, RiverCA riverMapCA, ForestCA forestMapCA, RessourceCA ressourceMapCA, SettlementCA settlementMapCA)
    {
        this.regionMapCA = regionMapCA;
        this.heightMapCA = heightMapCA;
        this.riverMapCA = riverMapCA;
        this.riverMapCA.Initialize(heightMapCA, regionMapCA);
        this.forestMapCA = forestMapCA;
        this.forestMapCA.Initialize(heightMapCA, regionMapCA, riverMapCA);
        this.ressourceMapCA = ressourceMapCA;
        this.ressourceMapCA.Initialize(heightMapCA, regionMapCA, riverMapCA, forestMapCA);
        this.settlementMapCA = settlementMapCA;
        this.settlementMapCA.Initialize(heightMapCA, regionMapCA, riverMapCA, ressourceMapCA, forestMapCA);
    }

    internal void UpdateRegionMap(int updateSteps)
    {
        regionMapCA.ApplySteps(updateSteps);
        RegionMapUpdatedEvent?.Invoke();
    }

    internal void UpdateHeightMap(int updateSteps)
    {
        heightMapCA.ApplySteps(updateSteps);
        HeightMapUpdatedEvent?.Invoke();
    }

    internal void UpdateRiverMap(int updateSteps)
    {
        if (!riverMapCA.Initialized)
        {
            riverMapCA.Initialize(heightMapCA, regionMapCA);
            riverMapCA.Initialized = true;
        }

        if(updateSteps > 1)
        {
            riverMapCA.UpdateUntilStable();
            RiverMapUpdatedEvent?.Invoke();
            HeightMapUpdatedEvent?.Invoke();
        }
        else
        {
            riverMapCA.ApplySteps(updateSteps);
            RiverMapUpdatedEvent?.Invoke();
            HeightMapUpdatedEvent?.Invoke();
        }
    }

    internal void UpdateForestMap(int updateSteps)
    {
        forestMapCA.ApplySteps(updateSteps);
        ForestMapUpdatedEvent?.Invoke();
    }

    internal void UpdateRessourceMap(int updateSteps)
    {
        ressourceMapCA.ApplySteps(updateSteps);
        RessourceMapUpdatedEvent?.Invoke();
    }

    internal bool TryRemoveRessource(Vector2Int ressourcePosition)
    {
        bool removed = ressourceMapCA.TryRemoveRessource(ressourcePosition);
        if (removed)
        {
            RessourceMapUpdatedEvent?.Invoke();
        }
        return removed;
    }

    internal void UpdateSettlementMap(int updateSteps)
    {
        settlementMapCA.ApplySteps(updateSteps);
        SettlementMapUpdatedEvent?.Invoke();
        ForestMapUpdatedEvent?.Invoke();
    }

    private CellularAutomaton<int> regionMapCA;
    private CellularAutomaton<int> heightMapCA;
    private RiverCA riverMapCA;
    private ForestCA forestMapCA;
    private RessourceCA ressourceMapCA;
    private SettlementCA settlementMapCA;
    internal event Action RegionMapUpdatedEvent;
    internal event Action HeightMapUpdatedEvent;
    internal event Action RiverMapUpdatedEvent;
    internal event Action ForestMapUpdatedEvent;
    internal event Action RessourceMapUpdatedEvent;
    internal event Action SettlementMapUpdatedEvent;

    internal CellularAutomaton<int> RegionMapCA
    {
        get
        {
            return regionMapCA;
        }
        set
        {
            regionMapCA = value;
            riverMapCA.RegionMapLayer = regionMapCA;
            forestMapCA.RegionMapLayer = regionMapCA;
            ressourceMapCA.RegionMapLayer = regionMapCA;
            settlementMapCA.RegionMapLayer = regionMapCA;
            RegionMapUpdatedEvent?.Invoke();
        }
    }

    internal CellularAutomaton<int> HeightMapCA
    {
        get
        {
            return heightMapCA;
        }
        set
        {
            heightMapCA = value;
            riverMapCA.HeightMapLayer = heightMapCA;
            forestMapCA.HeightMapLayer = heightMapCA;
            ressourceMapCA.HeightMapLayer = heightMapCA;
            settlementMapCA.HeightMapLayer = heightMapCA;
            HeightMapUpdatedEvent?.Invoke();
        }
    }

    internal RiverCA RiverMapCA
    {
        set
        {
            riverMapCA = value;
            forestMapCA.RiverMapLayer = riverMapCA;
            ressourceMapCA.RiverMapLayer = riverMapCA;
            settlementMapCA.RiverMapLayer = riverMapCA;
            RiverMapUpdatedEvent?.Invoke();
        }
    }

    internal ForestCA ForestMapCA
    {
        set
        {
            forestMapCA = value;
            ressourceMapCA.ForestMapLayer = forestMapCA;
            settlementMapCA.ForestMapLayer = forestMapCA;
            ForestMapUpdatedEvent?.Invoke();
        }
    }

    internal RessourceCA RessourceMapCA
    {
        set
        {
            ressourceMapCA = value;
            settlementMapCA.RessourceMapLayer = ressourceMapCA;
            RessourceMapUpdatedEvent?.Invoke();
        }
    }

    internal SettlementCA SettlementMapCA
    {
        set
        {
            settlementMapCA = value;
            SettlementMapUpdatedEvent?.Invoke();
        }
    }

    internal int MapWidth
    {
        get
        {
            return regionMapCA.MapDimensions.x;
        }
    }

    internal int MapHeight
    {
        get
        {
            return regionMapCA.MapDimensions.y;
        }
    }

    internal Dictionary<Vector2Int, int> MovementCostMap
    {
        get
        {
            Dictionary<Vector2Int, int> movementCostMap = new();
            foreach (KeyValuePair<Vector2Int, Cell<int>> entry in heightMapCA.MapState)
            {
                int movemnetCost = entry.Value.State > 1 ? 1 : 999;
                movementCostMap.Add(entry.Key, movemnetCost);
            }
            return movementCostMap;
        }
    }

    internal Dictionary<Vector2Int, int> RegionMapStates
    {
        get
        {
            Dictionary<Vector2Int, int> statesMap = new();
            foreach (KeyValuePair<Vector2Int, Cell<int>> keyValuePair in regionMapCA.MapState)
            {
                statesMap.Add(keyValuePair.Key, keyValuePair.Value.State);
            }
            return statesMap;
        }
    }

    internal Dictionary<Vector2Int, int> HeightMapStates
    {
        get
        {
            Dictionary<Vector2Int, int> statesMap = new();
            foreach (KeyValuePair<Vector2Int, Cell<int>> keyValuePair in heightMapCA.MapState)
            {
                statesMap.Add(keyValuePair.Key, keyValuePair.Value.State);
            }
            return statesMap;
        }
    }

    internal Dictionary<Vector2Int, HexWay> RiverMapStates
    {
        get
        {
            Dictionary<Vector2Int, HexWay> statesMap = new();
            foreach (KeyValuePair<Vector2Int, Cell<HexWay>> keyValuePair in riverMapCA.MapState)
            {
                statesMap.Add(keyValuePair.Key, keyValuePair.Value.State);
            }
            return statesMap;
        }
    }
    internal Dictionary<Vector2Int, int> RessourceMapStates
    {
        get
        {
            Dictionary<Vector2Int, int> statesMap = new();
            foreach (KeyValuePair<Vector2Int, Cell<int>> keyValuePair in ressourceMapCA.MapState)
            {
                statesMap.Add(keyValuePair.Key, keyValuePair.Value.State);
            }
            return statesMap;
        }
    }
    internal Dictionary<Vector2Int, bool> ForestMapStates
    {
        get
        {
            Dictionary<Vector2Int, bool> statesMap = new();
            foreach (KeyValuePair<Vector2Int, Cell<bool>> keyValuePair in forestMapCA.MapState)
            {
                statesMap.Add(keyValuePair.Key, keyValuePair.Value.State);
            }
            return statesMap;
        }
    }

    internal Dictionary<Vector2Int, bool> SettlementMapStates
    {
        get
        {
            Dictionary<Vector2Int, bool> statesMap = new();
            foreach (KeyValuePair<Vector2Int, Cell<float>> keyValuePair in settlementMapCA.MapState)
            {
                bool state = false;
                if(keyValuePair.Value.State == 1)
                {
                    state = true;
                }
                statesMap.Add(keyValuePair.Key, state);
            }
            return statesMap;
        }
    }

    internal List<Vector2Int> RiverSpringPositions
    {
        get
        {
            return riverMapCA.SpringPositions;
        }
    }
}
