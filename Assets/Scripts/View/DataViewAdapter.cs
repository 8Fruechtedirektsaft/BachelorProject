using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataViewAdapter : MonoBehaviour
{
    public void Start()
    {
        mapView = new(this, gameManager);
    }

    internal HexMapView Initialize()
    {
        return mapView;
    }

    internal void GenerateTileMapView()
    {
        Dictionary<Vector2Int, HexDebug> tileMap = new();
        foreach (KeyValuePair<Vector2Int, int> entry in hexMapData.RegionMap)
        {
            HexDebug mapTile = heightTilePrefabs[hexMapData.HeightMap[entry.Key]];
            if (hexMapData.HeightMap[entry.Key] > 1 && hexMapData.HeightMap[entry.Key] < 5)
            {
                mapTile = regionTilePrefabs[entry.Value];
            }
            mapTile.MapPosition = entry.Key;
            mapTile.State = entry.Value;
            Vector3 offset = Vector3.zero;
            if (hexMapData.HeightMap[entry.Key] > 1)
            {
                offset += heightOffset * (hexMapData.HeightMap[entry.Key] - 1);
            }
            tileMap.Add(entry.Key, Instantiate(mapTile, HexGridHelper.GridToWorld(entry.Key) + offset, Quaternion.identity));
        }
        mapView.TileMapLayer = tileMap;
    }

    internal void GenerateRegionMapView()
    {
        Dictionary<Vector2Int, HexDebug> tileMap = new();
        foreach (KeyValuePair<Vector2Int, int> entry in hexMapData.RegionMap)
        {
            HexDebug mapTile = regionTilePrefabs[entry.Value];
            mapTile.MapPosition = entry.Key;
            mapTile.State = entry.Value;
            tileMap.Add(entry.Key, Instantiate(mapTile, HexGridHelper.GridToWorld(entry.Key) + new Vector3(0,5,0), Quaternion.identity));
        }
        mapView.RegionMapLayer = tileMap;
    }

    internal void GenerateHeightMapView()
    {
        Dictionary<Vector2Int, HexDebug> tileMap = new();
        foreach (KeyValuePair<Vector2Int, int> entry in hexMapData.HeightMap)
        {
            HexDebug mapTile = heightTilePrefabs[entry.Value];
            mapTile.MapPosition = entry.Key;
            mapTile.State = entry.Value;
            Vector3 offSet = new(0,10,0);
            tileMap.Add(entry.Key, Instantiate(mapTile, HexGridHelper.GridToWorld(entry.Key) + offSet, Quaternion.identity));
        }
        mapView.HeightMapLayer = tileMap;
    }

    internal void ClearExtraMaps()
    {
        mapView.HeightMapLayer = new();
        mapView.RegionMapLayer = new();
    }

    internal void GenerateRiverMapView()
    {
        Dictionary<Vector2Int, List<GameObject>> riverTiles = new();
        foreach (KeyValuePair<Vector2Int, HexWay> entry in hexMapData.RiverMap)
        {
            List<GameObject> riverStreams = new();
            if (hexMapData.HeightMap[entry.Key] > 1)
            {
                foreach (Vector2Int direction in entry.Value.FromDirections)
                {
                    if (direction != Vector2Int.zero)
                    {
                        if (hexMapData.RiverSpringPositions.Contains(entry.Key))
                        {
                            Vector3 offset = new(0, 0.6f, 0);
                            if (hexMapData.HeightMap[entry.Key] > 1)
                            {
                                offset += heightOffset * (hexMapData.HeightMap[entry.Key] - 1);
                            }
                            GameObject riverTile = Instantiate(riverSpringPrefab, HexGridHelper.GridToWorld(entry.Key) + offset, Quaternion.identity);
                            riverStreams.Add(riverTile);
                        }
                        else
                        {
                            Quaternion rotation = Quaternion.Euler(0f, riverPrefabRotations[direction], 0f);
                            Vector3 offset = riverOffset;
                            if (hexMapData.HeightMap[entry.Key] > 1)
                            {
                                offset += heightOffset * (hexMapData.HeightMap[entry.Key] - 1);
                            }
                            GameObject riverTile = Instantiate(riverPrefab, HexGridHelper.GridToWorld(entry.Key) + offset, transform.rotation * rotation);
                            riverStreams.Add(riverTile);
                        }
                    }
                }
                if (entry.Value.ToDirection != Vector2Int.zero)
                {
                    Quaternion rotation = Quaternion.Euler(0f, riverPrefabRotations[entry.Value.ToDirection], 0f);
                    Vector3 offset = riverOffset;
                    if (hexMapData.HeightMap[entry.Key] > 1)
                    {
                        offset += heightOffset * (hexMapData.HeightMap[entry.Key] - 1);
                    }
                    GameObject riverTile = Instantiate(riverPrefab, HexGridHelper.GridToWorld(entry.Key) + offset, transform.rotation * rotation);
                    riverStreams.Add(riverTile);
                }
                riverTiles[entry.Key] = riverStreams;
            }
        }
        mapView.RiverMapLayer = riverTiles;
    }

    internal void GenerateForestMapView()
    {
        Dictionary<Vector2Int, HexDebug> forestTiles = new();
        foreach (KeyValuePair<Vector2Int, bool> entry in hexMapData.ForestMap)
        {
            if (entry.Value)
            {
                HexDebug forestTile = forestPrefabs[0];
                if (hexMapData.RegionMap[entry.Key] == 1)
                {
                    forestTile = forestPrefabs[1];
                }
                else if (hexMapData.RegionMap[entry.Key] == 3)
                {
                    forestTile = forestPrefabs[2];
                }

                Vector3 offset = new(0, 1.45f, 0);
                if (hexMapData.HeightMap[entry.Key] > 1)
                {
                    offset += heightOffset * (hexMapData.HeightMap[entry.Key] - 1);
                }

                forestTiles.Add(entry.Key, Instantiate(forestTile, HexGridHelper.GridToWorld(entry.Key) + offset, Quaternion.Euler(0, Random.Range(0, 360), 0)));
            }
        }
        mapView.ForestMapLayer = forestTiles;
    }

    internal void GenerateRessourceMapView()
    {
        Dictionary<Vector2Int, GameObject> ressourceMap = new();
        foreach (KeyValuePair<Vector2Int, int> entry in hexMapData.RessourceMap)
        {
            int ressourceState = entry.Value;
            if (ressourceState > 0)
            {
                GameObject ressourcePrefab = ressourcePrefabs[entry.Value - 1];
                Vector3 offset = new(0, 1f, 0);
                if (hexMapData.HeightMap[entry.Key] > 1)
                {
                    offset += heightOffset * (hexMapData.HeightMap[entry.Key] - 1);
                }
                ressourceMap.Add(entry.Key, Instantiate(ressourcePrefab, HexGridHelper.GridToWorld(entry.Key) + offset, Quaternion.identity));
            }
        }
        mapView.RessourceMapLayer = ressourceMap;
    }

    internal void GenerateSettlementMapView()
    {
        Dictionary<Vector2Int, GameObject> settlementTiles = new();
        foreach (KeyValuePair<Vector2Int, bool> entry in hexMapData.SettlementMap)
        {
            if (entry.Value)
            {
                Vector3 offset = new(0, -0.3f, 0);
                if (hexMapData.HeightMap[entry.Key] > 1)
                {
                    offset += heightOffset * (hexMapData.HeightMap[entry.Key] - 1);
                } 
                settlementTiles.Add(entry.Key, Instantiate(settlementPrefab, HexGridHelper.GridToWorld(entry.Key) + offset, Quaternion.Euler(0, Random.Range(0, 360), 0)));
            }
        }
        mapView.SettlementMapLayer = settlementTiles;
    }

    [SerializeField]
    private GameObject settlementPrefab;
    [SerializeField]
    private HexDebug[] mapTilePrefabs;
    [SerializeField]
    private HexDebug[] forestPrefabs;
    [SerializeField]
    private Material[] mapTileMaterials;
    [SerializeField]
    private GameObject riverPrefab;
    [SerializeField]
    private GameObject riverSpringPrefab;
    private readonly Dictionary<Vector2Int, float> riverPrefabRotations = new()
    {
        {new(-1, 0), 0 },
        {new(0, -1), 60 },
        {new(1, -1), 120 },
        {new(1, 0), 180 },
        {new(0, 1), 240 },
        {new(-1, 1), 300 },
    };
    private readonly Vector3 riverOffset = new(0, 0.291f, 0);
    [SerializeField]
    private GameObject[] ressourcePrefabs;
    [SerializeField]
    private HexDebug[] regionTilePrefabs;
    [SerializeField]
    private HexDebug[] heightTilePrefabs;
    private readonly Vector3 heightOffset = new(0, 0.7f, 0);
    [SerializeField]
    private HexMapData hexMapData;
    [SerializeField]
    private MyGameManager gameManager;

    private HexMapView mapView;
}
