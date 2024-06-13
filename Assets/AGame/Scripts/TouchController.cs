using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    public Transform characterTransform;
    public Collider2D characterCollider;
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
            isDragging = false;
            ChangeColor(Color.white);
        }

        if (isDragging)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 snappedPosition = SnapToGrid(mousePosition);
            head.position = snappedPosition;

            Vector2 direction = head.position - body.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            body.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            characterCollider.transform.position = characterTransform.position;
        }
    }

    Vector2 SnapToGrid(Vector2 originalPosition)
    {
        float gridSize = 1.0f;
        float snappedX = Mathf.Round(originalPosition.x / gridSize) * gridSize;
        float snappedY = Mathf.Round(originalPosition.y / gridSize) * gridSize;
        return new Vector2(snappedX, snappedY);
    }

    public void ChangeColor(Color newColor)
    {
        foreach (SpriteRenderer part in partsToColor)
        {
            part.color = newColor;
        }
    }
}
