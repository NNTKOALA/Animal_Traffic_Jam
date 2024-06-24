using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class TouchController : MonoBehaviour
{
    public Transform headPosition;
    public Transform bodyPosition;
    public SpriteRenderer[] partsToColor;
    public static TouchController currentActivePlayer = null;

    private bool isDragging = false;
    private bool isColliding = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public void Start()
    {
        initialPosition = headPosition.transform.position;
        initialRotation = bodyPosition.transform.rotation;
    }

    public void Update()
    {
        HandleInput();
    }

    public void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Input.mousePosition;
            GameObject clickedCharacter = GetCharacterUnderMouse(mousePosition);

            if (clickedCharacter != null && clickedCharacter.CompareTag("Player"))
            {
                TouchController clickedController = clickedCharacter.GetComponent<TouchController>();

                if (currentActivePlayer != null && currentActivePlayer != clickedController)
                {
                    currentActivePlayer.ChangeColor(Color.white);
                    currentActivePlayer.StopDragging();
                }

                currentActivePlayer = clickedController;
                currentActivePlayer.StartDragging();
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (currentActivePlayer != null && currentActivePlayer.isDragging)
            {
                currentActivePlayer.UpdateCharPosition();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (currentActivePlayer != null)
            {
                currentActivePlayer.StopDragging();
            }
        }
    }

    public GameObject GetCharacterUnderMouse(Vector2 mousePosition)
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    public void StartDragging()
    {
        isDragging = true;
        ChangeColor(new Color(1f, 0.91f, 0.73f));
        initialPosition = headPosition.position;
    }

    public void UpdateCharPosition()
    {
        if (!isColliding)
        {
            Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - bodyPosition.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            bodyPosition.rotation = rotation;
            initialPosition = headPosition.position;
        }
    }

    public void StopDragging()
    {
        isDragging = false;
        ChangeColor(Color.white);
        CheckTile();
    }

    public void CheckTile()
    {
        LayerMask tileLayerMask = LayerMask.GetMask("Tile");

        RaycastHit2D hit = Physics2D.Raycast(headPosition.position, Vector2.zero, Mathf.Infinity, tileLayerMask);

        if (hit.collider != null && hit.collider.CompareTag("Tile"))
        {
            Vector3 targetPosition = hit.collider.transform.position;
            Vector3 upVector = (targetPosition - transform.position).normalized;

            transform.up = upVector;
        }
        else
        {
            MoveToNearestTile();
        }
    }

    public void MoveToNearestTile()
    {
        Tile nearestTile = FindNearestAvailableTile();
        if (nearestTile != null)
        {
            headPosition.position = nearestTile.transform.position;
        }
    }

    public Tile FindNearestAvailableTile()
    {
        Tile[] tiles = FindObjectsOfType<Tile>();
        Tile nearestTile = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Tile tile in tiles)
        {
            if (!tile.isOccupied)
            {
                float distance = Vector3.Distance(bodyPosition.position, tile.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestTile = tile;
                }
            }
        }

        return nearestTile;
    }

    public void ChangeColor(Color newColor)
    {
        foreach (SpriteRenderer part in partsToColor)
        {
            part.color = newColor;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ChangeColor(new Color(243f / 255f, 128f / 255f, 128f / 255f));
            isColliding = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ChangeColor(Color.white);
            isColliding = false;
        }
    }
}