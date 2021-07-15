using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorSpots;
    [SerializeField]
    private Tilemap pathSpots;
    [SerializeField]
    private Tilemap TowerMap;

    [SerializeField]
    private TileBase selector;
    [SerializeField]
    private TileBase meleebase;
    [SerializeField]
    private TileBase turretbase;
    [SerializeField]
    private TileBase dendritic;
    [SerializeField]
    private TileBase hemocytoblast;
    [SerializeField]
    private TileBase mastcell;

    [SerializeField]
    private GameObject BCellTurretPrefab;

    Color floorColor = new Color(0.227f, 0.004f, 0f, 1f);
    Color pathColor = new Color(0.478f, 0.196f, 0.161f, 1f);

    private enum TowerID
    {
        None = 0,   // this probably shouldn't be used for anything; empty cells should have their dict entry removed

        // melee units (id < 0)
        Neutrophil = -1,
        Macrophage = -2,

        // turret / aoe units (id > 0)
        BCell = 1,
        TCell = 2,
        NKCell = 3,
        DendriticCell = 4,
        Hemocytoblast = 5,
        MastCell = 6
    }

    private Dictionary<Vector3Int, TowerID> Towers = new Dictionary<Vector3Int, TowerID>(); // dict of all placed towers with coords as key

    public int RequestSpawn(int id, Vector3Int pos)
    {
        if (Towers.ContainsKey(pos)){       // is this tile already occupied?
            return -1;
        } else if (id < 0)                  // is the tower we want to spawn a melee unit?
        {
            if (pathSpots.GetTile(pos) == selector)     // is the spot a valid tile for a melee unit?
            {
                Towers.Add(pos, (TowerID)id);   // cast from int id to towerid enum
                RefreshTower(pos);
                return 1;
            }
            else
            {
                return 0;
            }
        } else if (id > 0)                  // is the tower we want to spawn a turret unit?
        {
            if (floorSpots.GetTile(pos) == selector)    // is the spot a valid tile for a turret unit?
            {
                Towers.Add(pos, (TowerID)id);
                RefreshTower(pos);
                return 1;
            }
            else
            {
                return 0;
            }
        } else
        {
            return -2;
        }
        /* return codes:
         * -2: attempting to spawn None or other unspecified error
         * -1: tile already occupied
         *  0: invalid tile for unit
         *  1: success
         */
    }

    private void RefreshTower(Vector3Int pos)    // update tower tile at pos based on dictionary state
    {
        if (Towers.ContainsKey(pos))        // should there be a tower at pos?
        {
            switch (Towers[pos])
            {
                case TowerID.Neutrophil:
                    TowerMap.SetTile(pos, meleebase);
                    break;
                case TowerID.Macrophage:
                    TowerMap.SetTile(pos, meleebase);
                    TowerMap.SetColor(pos, new Color(0.486f, 0.322f, 0.678f, 1f));
                    break;
                case TowerID.BCell:
                    TowerMap.SetTile(pos, turretbase);
                    TowerMap.SetColor(pos, new Color(0.231f, 0.416f, 0.937f, 1f));
                    GameObject.Instantiate(BCellTurretPrefab, (TowerMap.CellToWorld(pos) + new Vector3(0.5f, 0.5f, 0f)), Quaternion.identity);
                    break;
                case TowerID.TCell:
                    TowerMap.SetTile(pos, turretbase);
                    TowerMap.SetColor(pos, new Color(0.937f, 0.744f, 0.231f, 1f));
                    break;
                case TowerID.NKCell:
                    TowerMap.SetTile(pos, turretbase);
                    TowerMap.SetColor(pos, new Color(0.29f, 0.29f, 0.29f, 1f));
                    break;
                case TowerID.DendriticCell:
                    TowerMap.SetTile(pos, dendritic);
                    break;
                case TowerID.Hemocytoblast:
                    TowerMap.SetTile(pos, hemocytoblast);
                    break;
                case TowerID.MastCell:
                    TowerMap.SetTile(pos, mastcell);
                    break;
                default:
                    return;
            }
            // if we just exited this switch, we can assume that we just spawned a valid tower, so let's hide the selector tile
            if((int)Towers[pos] < 0)    // is this a melee unit?
            {
                pathSpots.SetColor(pos, Color.clear);
                pathSpots.RefreshTile(pos);     // we need to refresh the tile so MapManager can see its color
            } else                      // is this a turret unit?
            {
                floorSpots.SetColor(pos, Color.clear);
                floorSpots.RefreshTile(pos);
            }
        }
        else      // dict has no entry at this pos, so if there's a tower here, we should delete it
        {
            TowerMap.DeleteCells(pos, Vector3Int.one);
            // and let's unhide the selector tile here:
            pathSpots.SetColor(pos, pathColor);
            pathSpots.RefreshTile(pos);
            floorSpots.SetColor(pos, floorColor);
            floorSpots.RefreshTile(pos);
            // faster to just call SetColor on both tilemaps than check which it's supposed to be; there'll only be one at a given pos
        }
    }
}