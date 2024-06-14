using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    public Transform character;
    public Transform head;
    public Transform body;
    public SpriteRenderer[] partsToColor;
    public GridManager gridManager;
    private bool isDragging = false;

    void Start()
    {
        
    }

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
            isDragging = false;
            ChangeColor(Color.white);
        }

        if (isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;

            Vector3 snappedPosition = SnapToGrid(mousePosition);

            Vector3 direction = (snappedPosition  -  transform.position).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            character.rotation = Quaternion.Euler(0, 0, angle);
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
}
