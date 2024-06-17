using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Tile[] tiles;
    public List<TouchController> touchControllers;
    public float snapRange = 0.5f;

    private void Start()
    {
        foreach (TouchController script in touchControllers)
        {
            script.dargEndedDelegate = SnapObject;
        }

        foreach (Tile tile in tiles)
        {
            Vector2Int gridPos = WorldToGridPosition(tile.transform.position);
            tile.Initialize(gridPos);
        }
    }

    public void SnapObject(Transform obj)
    {
        foreach (Tile tile in tiles)
        {
            if (Vector2.Distance(tile.transform.position, obj.position) <= snapRange)
            {
                obj.position = tile.transform.position;
            }
        }
    }

    public Vector2Int WorldToGridPosition(Vector2 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x);
        int y = Mathf.FloorToInt(worldPosition.y);
        return new Vector2Int(x, y);
    }

    public Tile GetTileAtPosition(Vector2Int gridPos)
    {
        foreach (Tile tile in tiles)
        {
            if (tile.gridPosition == gridPos)
            {
                return tile;
            }
        }
        return null;
    }

    public bool IsTileOccupied(Vector2Int gridPos)
    {
        Tile tile = GetTileAtPosition(gridPos);
        return tile != null && tile.isOccupied;
    }

    public void OccupyTile(Vector2Int gridPos, bool occupied)
    {
        Tile tile = GetTileAtPosition(gridPos);
        if (tile != null)
        {
            tile.isOccupied = occupied;
        }
    }
}