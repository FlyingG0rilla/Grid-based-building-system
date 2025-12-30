using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileParent : MonoBehaviour
{
    public static TileParent instance;

    [SerializeField] private int gridSizeX;
    [SerializeField] private int gridSizeZ;
    [SerializeField] Grid grid;
    [SerializeField] GameObject tilePrefab;

    [SerializeField] public Dictionary<Vector3, GameTile> tiles;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        GenerateGrid();
        
    }

    public Vector3 WorldLocationToGridCordinates(Vector3 worldLocation)
    {
        return grid.WorldToCell(worldLocation);
    }

    private void GenerateGrid()
    {
        tiles = new Dictionary<Vector3, GameTile>();

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                var worldPosition = grid.GetCellCenterWorld(new Vector3Int(x, z));

                Vector3Int localPlace = grid.WorldToCell(worldPosition);

                var tile = new GameTile
                {
                    LocalPlace = localPlace,
                    WorldLocation = worldPosition,
                    Name = localPlace.x + "," + localPlace.y,
                    isEmpty = true,                
                };

                tiles.Add(tile.LocalPlace, tile);
              
                
            }
        }
        Debug.Log("GridGenerated");
    }
}
