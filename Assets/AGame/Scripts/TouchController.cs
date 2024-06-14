using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    public GridManager gridManager;
    public Transform head;
    public Transform body;
    private bool isDragging = false;
    public SpriteRenderer[] partsToColor;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Vector2.Distance(mousePosition, head.position) < 0.5f)
            {
                isDragging = true;
                ChangeColor(new Color(1f, 0.91f, 0.73f));
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2Int gridPos = gridManager.WorldToGridPosition(mousePosition);
                Tile targetTile = gridManager.GetTileAtPosition(gridPos);

                if (targetTile != null && !targetTile.isOccupied)
                {
                    head.position = targetTile.transform.position;
                    targetTile.isOccupied = true;
                }

                ChangeColor(Color.white);
            }
            isDragging = false;
        }

        if (isDragging)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 snappedPosition = SnapToGrid(mousePosition);
            head.position = snappedPosition;

            Vector2 direction = head.position - body.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            body.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    Vector2 SnapToGrid(Vector2 originalPosition)
    {
        Vector2Int gridPos = gridManager.WorldToGridPosition(originalPosition);
        Tile tile = gridManager.GetTileAtPosition(gridPos);
        if (tile != null)
        {
            return tile.transform.position;
        }
        return originalPosition;
    }

    public void ChangeColor(Color newColor)
    {
        foreach (SpriteRenderer part in partsToColor)
        {
            part.color = newColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            ChangeColor(new Color(243f / 255f, 128f / 255f, 128f / 255f));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            ChangeColor(Color.white);
        }
    }
}
