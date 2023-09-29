using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HexMapView 
{
    public HexMapView(DataViewAdapter dataViewAdapter, MyGameManager gameManager)
    {
        this.dataViewAdapter = dataViewAdapter;
        this.gameManager = gameManager;
        tileMapLayer = new();
        regionMapLayer = new();
        ressourceMapLayer = new();
        heightMapLayer = new();
        riverMapLayer = new();
        forestMapLayer = new();
        settlementMapLayer = new();
    }

    internal readonly MyGameManager gameManager;
    internal readonly DataViewAdapter dataViewAdapter;

    private Dictionary<Vector2Int, HexDebug> tileMapLayer;
    internal Dictionary<Vector2Int, HexDebug> TileMapLayer
    {
        set
        {
            foreach (HexDebug tile in tileMapLayer.Values)
            {
                GameObject.Destroy(tile.gameObject);
            }
            tileMapLayer = value;
        }
    }

    private Dictionary<Vector2Int, HexDebug> regionMapLayer;
    internal Dictionary<Vector2Int, HexDebug> RegionMapLayer
    {
        set
        {
            foreach (HexDebug tile in regionMapLayer.Values)
            {
                GameObject.Destroy(tile.gameObject);
            }
            regionMapLayer = value;
        }
    }


    private Dictionary<Vector2Int, HexDebug> heightMapLayer;
    internal Dictionary<Vector2Int, HexDebug> HeightMapLayer
    {
        set
        {
            foreach (HexDebug tile in heightMapLayer.Values)
            {
                GameObject.Destroy(tile.gameObject);
            }
            heightMapLayer = value;
        }
    }

    private Dictionary<Vector2Int, List<GameObject>> riverMapLayer;
    internal Dictionary<Vector2Int, List<GameObject>> RiverMapLayer
    {
        set
        {
            foreach (List<GameObject> rivers in riverMapLayer.Values)
            {
                foreach (GameObject river in rivers)
                {
                    GameObject.Destroy(river);
                }
            }
            riverMapLayer = value;
        }
    }

    private Dictionary<Vector2Int, HexDebug> forestMapLayer;
    internal Dictionary<Vector2Int, HexDebug> ForestMapLayer
    {
        set
        {
            foreach (HexDebug tile in forestMapLayer.Values)
            {
                GameObject.Destroy(tile.gameObject);
            }
            forestMapLayer = value;
        }
    }

    private Dictionary<Vector2Int, GameObject> ressourceMapLayer;
    internal Dictionary<Vector2Int, GameObject> RessourceMapLayer
    {
        set
        {
            foreach (GameObject ressource in ressourceMapLayer.Values)
            {
                GameObject.Destroy(ressource);
            }
            ressourceMapLayer = value;
        }
    }

    private Dictionary<Vector2Int, GameObject> settlementMapLayer;
    internal Dictionary<Vector2Int, GameObject> SettlementMapLayer
    {
        set
        {
            foreach (GameObject settlement in settlementMapLayer.Values)
            {
                GameObject.Destroy(settlement);
            }
            settlementMapLayer = value;
        }
    }
}
