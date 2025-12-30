using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameTile 
{
    public Vector3Int LocalPlace { get; set; }

    public Vector3 WorldLocation { get; set; }

    public string Name { get; set; }

    public bool isEmpty { get; set; }

    public PlacableSO structureOnTile { get; set; }

    public TileType tileType;

    public enum TileType
    {
        WallStructure,
        TowerStructure,
        ResourceStructure,
        Default
    }
}
