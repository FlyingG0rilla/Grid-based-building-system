using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlacableSO : ScriptableObject
{
    public GameObject Prefab;

    public GameObject PreviewPrefab;

    public int sizeX;

    public int sizeZ;

    public bool isWall;

    public bool isPlacableOnWall;

    public bool isResourceBuilding;
}
