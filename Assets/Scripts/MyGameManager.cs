using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameManager : MonoBehaviour
{
    public void Start()
    {
        Application.targetFrameRate = 10;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            hexMapData.UpdateRegionMapOnly(6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            hexMapData.UpdateHeightMapOnly(12);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            hexMapData.UpdateRiverMap(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            hexMapData.UpdateForestMap(hexMapData.MapDimensions.x * hexMapData.MapDimensions.y);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            hexMapData.UpdateRessourceMap(hexMapData.MapDimensions.x * hexMapData.MapDimensions.y);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            hexMapData.UpdateSettlementMap(hexMapData.MapDimensions.x * hexMapData.MapDimensions.y * 6);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            hexMapData.UpdateRegionMapOnly(1);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            hexMapData.UpdateHeightMapOnly(1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            hexMapData.UpdateRiverMap(1);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            hexMapData.UpdateForestMap(hexMapData.MapDimensions.x * hexMapData.MapDimensions.y / 10);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            hexMapData.UpdateRessourceMap(hexMapData.MapDimensions.x * hexMapData.MapDimensions.y / 10);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            hexMapData.UpdateSettlementMap(hexMapData.MapDimensions.x * hexMapData.MapDimensions.y);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            hexMapData.ClearOnlyMaps();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            int removedCount = 0;
            do
            {
                int x = UnityEngine.Random.Range(0, hexMapData.MapDimensions.x);
                int y = UnityEngine.Random.Range(0, hexMapData.MapDimensions.y);

                if (hexMapData.TryRemoveRessource(new(x, y)))
                {
                    removedCount++;
                }
            } while (removedCount < 10);
        }

    }

    [SerializeField]
    private HexMapData hexMapData;
    [SerializeField]
    private DataViewAdapter dataViewAdapter;
}

