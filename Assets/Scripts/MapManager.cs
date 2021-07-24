using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorSpots;
    [SerializeField]
    private Tilemap pathSpots;
    [SerializeField]
    private TilemapRenderer floorRenderer;
    [SerializeField]
    private TilemapRenderer pathRenderer;
    [SerializeField]
    private TowerManager tm;
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private Vector3Int[] gridPosFloor = new Vector3Int[2];
    [SerializeField]
    private Vector3Int[] gridPosPath = new Vector3Int[2];
    Color floorColor = new Color(0.227f, 0.004f, 0f, 1f);
    Color pathColor = new Color(0.478f, 0.196f, 0.161f, 1f);
    Color green = new Color(0.125f, 0.643f, 0.306f, 1f);

    enum PlaceMode
    {
        None,   // nothing is currently being placed
        Turret, // we are attempting to place a turret tower
        Melee   // we are attempting to place a melee tower
    }
    [SerializeField]
    private PlaceMode currentPlaceMode = PlaceMode.None;
    private int PlaceID;

    private void Update()
    {   
        if (currentPlaceMode == PlaceMode.Turret)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gridPosFloor[1] = floorSpots.WorldToCell(mousePos);
            if(gridPosFloor[1] != gridPosFloor[0])                          // are we hovering over a new cell?
            {
                if (gridPosFloor[0] != null)
                {
                    if (floorSpots.GetColor(gridPosFloor[0]) != Color.clear)    // make sure we're not changing color of a hidden cell
                    {
                        floorSpots.SetColor(gridPosFloor[0], floorColor);   // reset the color of the previous cell
                    }
                }
                if (floorSpots.GetColor(gridPosFloor[1]) != Color.clear)
                {
                    floorSpots.SetColor(gridPosFloor[1], green);            // change color of cell we're currently hovering over
                }
                gridPosFloor[0] = gridPosFloor[1];                          // save current cell position to reference next time
            }
            if (Input.GetMouseButtonDown(0))
            {
                int requestResult = tm.RequestSpawn(PlaceID, gridPosFloor[1]);
                if(requestResult == 1)           // attempt to spawn turret, reset place mode upon success
                {
                    uiManager.HidePlacement();
                    CancelPlace();
                } else if (requestResult == -2)     // were we unable to afford the tower?
                {
                    uiManager.ShowPlacement();
                    CancelPlace();
                }
            }
        }
        if (currentPlaceMode == PlaceMode.Melee)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gridPosPath[1] = pathSpots.WorldToCell(mousePos);
            if (gridPosPath[1] != gridPosPath[0])
            {
                if (gridPosPath[0] != null)
                {
                    if (pathSpots.GetColor(gridPosPath[0]) != Color.clear)
                    {
                        pathSpots.SetColor(gridPosPath[0], pathColor);
                    }
                }
                if (pathSpots.GetColor(gridPosPath[1]) != Color.clear)
                {
                    pathSpots.SetColor(gridPosPath[1], green);
                }
                gridPosPath[0] = gridPosPath[1];
            }
            if (Input.GetMouseButtonDown(0))
            {
                if(tm.RequestSpawn(PlaceID, gridPosPath[1]) == 1)
                {
                    uiManager.HidePlacement();
                    CancelPlace();
                }
            }
        }
    }

    private void UpdatePlaceMode()  // hide/show tilemaps based on currentPlaceMode
    {
        switch (currentPlaceMode)
        {
            case PlaceMode.None:
                floorRenderer.enabled = false;
                pathRenderer.enabled = false;
                break;
            case PlaceMode.Turret:
                floorRenderer.enabled = true;
                pathRenderer.enabled = false;
                break;
            case PlaceMode.Melee:
                floorRenderer.enabled = false;
                pathRenderer.enabled = true;
                break;
        }
    }

    public void RequestPlace(int id)
    {
        /* Neutrophil = -1,
         * Macrophage = -2,
         * BCell = 1,
         * TCell = 2,
         * NKCell = 3,
         * DendriticCell = 4,
         * Hemocytoblast = 5,
         * MastCell = 6,
         * (negative: melee, positive: turret)
         */
        if (id < 0)
        {
            currentPlaceMode = PlaceMode.Melee;
            uiManager.DuringPlacement();
        } else if (id > 0)
        {
            currentPlaceMode = PlaceMode.Turret;
            uiManager.DuringPlacement();
        }
        else
        {
            return;
        }
        UpdatePlaceMode();
        PlaceID = id;
    }

    public void CancelPlace()
    {
        currentPlaceMode = PlaceMode.None;
        UpdatePlaceMode();
    }
}

