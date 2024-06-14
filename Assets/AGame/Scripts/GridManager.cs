using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Tile[] tiles;

    void Start()
    {
        InitializeTiles();
    }

    void InitializeTiles()
    {
        foreach (Tile tile in tiles)
        {
            Vector3 worldPos = tile.transform.position;
            Vector2Int gridPos = WorldToGridPosition(worldPos);
            tile.Initialize(gridPos);
        }
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

    public Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        float gridSize = .25f;
        int x = Mathf.FloorToInt(worldPos.x / gridSize);
        int y = Mathf.FloorToInt(worldPos.y / gridSize);
        return new Vector2Int(x, y);
    }

    public Tile GetNearestTile(Vector3 worldPos)
    {
        Tile nearestTile = null;
        float minDistance = float.MaxValue;

        foreach (Tile tile in tiles)
        {
            float distance = Vector3.Distance(worldPos, tile.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTile = tile;
            }
        }
        return nearestTile;
    }
}
