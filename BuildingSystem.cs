using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem current;

    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask placementLayerMask;
    [SerializeField] public Grid grid;
    [SerializeField] private GameObject gridVisual;

    //Temp
    [SerializeField] private Material validPosition;
    [SerializeField] private Material invalidPosition;

    private PlacableSO objectToPlace;
    private GameObject activePreview = null;
    private bool isPreviewActive = false;
    private bool isBuildingMode = false;
    private int rotationFactor = 0;

    private Vector3 lastPosition;

    private GameTile _tile;


    private void Awake()
    {
        current = this;
        rotationFactor = 0;
    }

    void Update()
    {

        //Temp
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopPlacement();
        }

        if(isPreviewActive == true)
        {
            UpdatePreview();
        }

        if(isBuildingMode == true && Input.GetMouseButtonDown(0))
        {
            PlaceStructure(objectToPlace);
        }  

        if(isBuildingMode && Input.GetKeyDown(KeyCode.R))
        {
            RotateStructure();
        }
    }



    public Vector3 GetSelectedMapPosition()
    {
        //Input from Mouse to Vector 3 (Old InputSystem)
        Vector3 mousePos = Input.mousePosition;

        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, placementLayerMask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }

    public Vector3 WorldPositionToGridPosition(Vector3 positionToConvert)
    {
        Vector3 posToReturn = grid.WorldToCell(positionToConvert);
        return posToReturn;
    }

    public void StartPlacement(PlacableSO _objectToPlace)
    {       
        StopPlacement();

        objectToPlace = _objectToPlace;

        isBuildingMode = true;
        EnablePreview();
    }

    private void StopPlacement()
    {
        isBuildingMode = false;
        objectToPlace = null;
        rotationFactor = 0;
        DisablePreview();     
    }

    private void PlaceStructure(PlacableSO placableToPlace)
    {      
        
        Vector3 mousePosition = GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        Vector3 objPosition = grid.CellToWorld(gridPosition);
        objPosition.y = mousePosition.y;

      
        Vector3 [] tilesToCheck = CalculateTilesToOcupie(objectToPlace.sizeX, objectToPlace.sizeZ, objPosition);
    
        bool canPlace = CheckTilesOnGrid(tilesToCheck);
        bool canOverlap = CheckOverlapPlacement(tilesToCheck);

        if (canPlace == true || canOverlap == true)
        {
            DisablePreview();
;
            OcupieTilesOnGrid(tilesToCheck);
            Transform newObject = Instantiate(placableToPlace.Prefab.transform);
            newObject.transform.position = objPosition;
            newObject.transform.eulerAngles = new Vector3(0f, rotationFactor, 0f);


            var tiles = TileParent.instance.tiles; // This is our Dictionary of tiles

            for (int i = 0; i < tilesToCheck.Length; i++)
            {
                Vector3 tile = grid.WorldToCell(tilesToCheck[i]);
                GameTile gameTile = tiles[tile];
                gameTile.structureOnTile = objectToPlace;
            }

            Debug.Log(grid.WorldToCell(objPosition));

            StopPlacement();
        }

    }

    private void RotateStructure()
    {
        if (isBuildingMode == true)
        {
            rotationFactor = rotationFactor + 90;

            if (rotationFactor > 270)
                rotationFactor = 0;
        }
    }

    private void OcupieTilesOnGrid(Vector3[] tilesToCheck)
    {
        var tiles = TileParent.instance.tiles; // This is our Dictionary of tiles

        for (int i = 0; i < tilesToCheck.Length; i++)
        {
            Vector3 tile = grid.WorldToCell(tilesToCheck[i]);

            tiles[tile].isEmpty = false;
            Debug.Log("Set Tile false" + tile);         
        }

        if(objectToPlace.isWall == true)
        {
            for(int i = 0; i < tilesToCheck.Length; i++)
            {
                Vector3 tile = grid.WorldToCell(tilesToCheck[i]);
                tiles[tile].tileType = GameTile.TileType.WallStructure;
            }
        }

        Debug.Log("SetTilesInArray");
    }

    private bool CheckTilesOnGrid(Vector3[] tilesToCheck) 
    {
        var tiles = TileParent.instance.tiles; // This is our Dictionary of tiles

        for (int i = 0; i < tilesToCheck.Length; i++)
        {
            Vector3 tile = grid.WorldToCell(tilesToCheck[i]);

            //Check weather or not tile is even in dictionary // aka valid Building area selected
            if(tiles.ContainsKey(tile) == false)
                return false;

            Debug.Log(tiles[tile].Name);


            if (tiles[tile].isEmpty == false)
            {
                Debug.Log("Tile Not empty");
                return false;
            }
            else
            {
                Debug.Log("Tile is empty");
            }
        }
        return true;
    }
    

    public Vector3[] CalculateTilesToOcupie(int sizeX, int sizeZ, Vector3 position)
    {
        Vector3 [] TilesToOccupie = new Vector3[sizeX * sizeZ];

        Vector3 cellSize = BuildingSystem.current.grid.cellSize;

        int i = 0;

        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {

                /*
                var rotation = Quaternion.AngleAxis(rotationFactor, position);
                position = rotation * position;
                //TilesToOccupie[i] = position + new Vector3(cellSize.x * x, 0f, 0f) + new Vector3(0f, 0f, cellSize.y * z);
                */


                //Sub Optimalle Lösung
                //Alternativ Position lokal rotieren
                if (rotationFactor == 0)
                {
                    TilesToOccupie[i] = position + new Vector3(cellSize.x * x, 0f, 0f) + new Vector3(0f, 0f, cellSize.y * z);
                    Debug.Log("Calc 1");
                }
                else if (rotationFactor == 90)
                {
                    TilesToOccupie[i] = position - new Vector3(0f, 0f, cellSize.x * x) + new Vector3(cellSize.y * z, 0f, 0f);
                    Debug.Log("Calc 2");
                }
                else if (rotationFactor == 180)
                {
                    TilesToOccupie[i] = position - new Vector3(cellSize.x * x, 0f, 0f) - new Vector3(0f, 0f, cellSize.y * z);
                    Debug.Log("Calc 3");
                }
                else if (rotationFactor == 270)
                {
                    TilesToOccupie[i] = position + new Vector3(0f, 0f, cellSize.x * x) - new Vector3(cellSize.y * z, 0f, 0f);
                    //TilesToOccupie[i] = position - new Vector3(cellSize.x * x, 0f, 0f) + new Vector3(0f, 0f, cellSize.y * z);
                    Debug.Log("Calc 4");
                }

                //TilesToOccupie[i] = position + new Vector3(cellSize.x * x, 0f, 0f) + new Vector3(0f, 0f, cellSize.y * z);
                i++;
            }         
        }

        return TilesToOccupie;
    }

    private bool CheckOverlapPlacement(Vector3[] tilesToCheck)
    {
        var tiles = TileParent.instance.tiles; // This is our Dictionary of tiles

        for (int i = 0; i < tilesToCheck.Length; i++)
        {
            Vector3 tile = grid.WorldToCell(tilesToCheck[i]);

            GameTile gameTile = tiles[tile];
            if(gameTile.tileType == GameTile.TileType.WallStructure)
            {
                if(objectToPlace.isPlacableOnWall == true)
                {
                    return true;
                }
            }

        }

        return false;
    }

    #region PreviewSystem
    private void EnablePreview()
    {
        Vector3 mousePosition = GetSelectedMapPosition();
        Transform previewObject = Instantiate(objectToPlace.PreviewPrefab.transform);
        previewObject.transform.position = mousePosition + new Vector3(0, 0.1f, 0);
        activePreview = previewObject.gameObject;

        isPreviewActive = true;  

        gridVisual.GetComponent<Renderer>().enabled = isPreviewActive;
    }

    private void DisablePreview()
    {
        Destroy(activePreview);

        isPreviewActive = false;
        activePreview = null;

        gridVisual.GetComponent<Renderer>().enabled = isPreviewActive;
    }

    private void UpdatePreview()
    {
        if (activePreview != null)
        {
            Vector3 currentMousePos = GetSelectedMapPosition();
            Vector3Int gridPosition = grid.WorldToCell(currentMousePos);

            Vector3 previewPosition = grid.CellToWorld(gridPosition) + new Vector3(0, 0.5f, 0);
            previewPosition.y = currentMousePos.y + 0.5f;
            activePreview.transform.position = previewPosition;

            //activePreview.transform.position = grid.CellToWorld(gridPosition) + new Vector3(0, 0.5f, 0);
            //activePreview.transform.position.y = currentMousePos.y + 0.5;
            activePreview.transform.eulerAngles = new Vector3(0f, rotationFactor, 0f);

           

            //Test
            Vector3 objPosition = grid.CellToWorld(gridPosition);

            Vector3[] tilesToCheck = CalculateTilesToOcupie(objectToPlace.sizeX, objectToPlace.sizeZ, objPosition);

            bool canPlace = CheckTilesOnGrid(tilesToCheck);
            bool canOverlap = CheckOverlapPlacement(tilesToCheck);

            if (canPlace || canOverlap) // rework later 
            {
                Renderer[] previewChildren = activePreview.GetComponentsInChildren<Renderer>();
                foreach (Renderer i in previewChildren)
                {
                    i.material = validPosition;
                }
            }
            else
            {
                Renderer[] previewChildren = activePreview.GetComponentsInChildren<Renderer>();
                foreach (Renderer j in previewChildren)
                {
                    j.material = invalidPosition;
                }
            }
        }
    }


    #endregion
}
