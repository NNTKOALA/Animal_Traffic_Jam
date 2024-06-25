using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class TouchController : MonoBehaviour
{
    public Transform headPosition;
    public Transform bodyPosition;
    public Transform point1Position;
    public Transform point2Position;
    public SpriteRenderer[] partsToColor;
    public static TouchController currentActivePlayer = null;
    public float moveSpeed = 5f;
    public float mapBoundaryY = 10f;

    private Collider2D charCollider;
    private bool isDragging = false;
    private bool isColliding = false;

    public void Start() 
    {
        charCollider = GetComponent<Collider2D>();
    }

    public void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Input.mousePosition;
            GameObject clickedCharacter = GetCharacterUnderMouse(mousePosition);

            if (clickedCharacter != null && clickedCharacter.CompareTag("Object"))
            {
                TouchController clickedController = clickedCharacter.GetComponent<TouchController>();
                clickedCharacter.tag = "Player";

                if (currentActivePlayer != null && currentActivePlayer != clickedController)
                {
                    currentActivePlayer.ChangeCharColor(Color.white);
                    currentActivePlayer.StopDragging();
                }

                currentActivePlayer = clickedController;
                currentActivePlayer.StartDragging();
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (currentActivePlayer != null && currentActivePlayer.isDragging)
            {
                currentActivePlayer.UpdateCharPosition();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (currentActivePlayer != null)
            {
                currentActivePlayer.StopDragging();
                if (!isColliding)
                {
                    currentActivePlayer.EscapingMovement();
                }
                currentActivePlayer.gameObject.tag = "Object";
                currentActivePlayer = null;
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
        ChangeCharColor(new Color(1f, 0.91f, 0.73f));
    }

    public void UpdateCharPosition()
    {
        if (!isColliding)
        {
            Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - bodyPosition.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            bodyPosition.rotation = rotation;
        }
    }

    public void StopDragging()
    {
        isDragging = false;
        ChangeCharColor(Color.white);
        CheckTile();
    }

    public void CheckTile()
    {
        LayerMask tileLayerMask = LayerMask.GetMask("Tile");
        RaycastHit2D[] hits = new RaycastHit2D[5];

        hits[0] = Physics2D.Raycast(headPosition.position, Vector2.up, .5f, tileLayerMask);
        hits[1] = Physics2D.Raycast(headPosition.position, Vector2.right, .5f, tileLayerMask);
        hits[2] = Physics2D.Raycast(headPosition.position, Vector2.left, .5f, tileLayerMask);
        hits[3] = Physics2D.Raycast(headPosition.position, Vector2.down, .5f, tileLayerMask);

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Tile"))
            {
                Vector3 targetPosition = hit.collider.transform.position;
                Vector3 upVector = (targetPosition - transform.position).normalized;
                transform.up = upVector;
                return;
            }
        }
        MoveToNearestTile();
    }

    public void MoveToNearestTile()
    {
        Tile nearestTile = FindNearestAvailableTile();
        if (nearestTile != null)
        {
            Vector3 targetPosition = nearestTile.transform.position;
            Vector3 upVector = (targetPosition - transform.position).normalized;
            transform.up = upVector;
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
                float distance = Vector3.Distance(headPosition.position, tile.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestTile = tile;
                }
            }
        }

        return nearestTile;
    }

    public void EscapingMovement()
    {
        LayerMask tileLayerMask = LayerMask.GetMask("Player");

        float distanceBetweenPoints = Vector2.Distance(point1Position.position, point2Position.position);

        Vector2 midPoint = (point1Position.position + point2Position.position) / 2;

        RaycastHit2D hit = Physics2D.BoxCast(midPoint, new Vector2(distanceBetweenPoints, 0.1f), 0, Vector2.up, Mathf.Infinity, tileLayerMask);

        if (hit.collider != null)
        {
            Debug.Log("Hit: " + hit.collider.name + ", Tag: " + hit.collider.tag);

            if (hit.collider.CompareTag("Object"))
            {
                ChangeCharColor(hit.collider);
                CheckTile();
                return;
            }
            else
            {
                StartCoroutine(MoveCharacterOutsideMap());
            }
        }
        else
        {
            Debug.Log("No hit detected");
            isColliding = false;
            StartCoroutine(MoveCharacterOutsideMap());
        }
    }

    private IEnumerator MoveCharacterOutsideMap()
    {
        Vector2 direction = (headPosition.position - bodyPosition.position).normalized;
        Debug.Log("Starting movement outside map.");

        if (charCollider != null)
        {
            charCollider.enabled = false;
        }

        while (bodyPosition.position.y < mapBoundaryY)
        {
            bodyPosition.position += (Vector3)direction * moveSpeed * Time.deltaTime;
            yield return null;
        }

        Debug.Log("Character has moved outside the map.");
    }

    private void ChangeCharColor(Collider2D collider)
    {
        if (collider != null)
        {
            SpriteRenderer spriteRenderer = collider.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(243f / 255f, 128f / 255f, 128f / 255f);
            }
        }
    }

    public void ChangeCharColor(Color newColor)
    {
        foreach (SpriteRenderer part in partsToColor)
        {
            part.color = newColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Object"))
        {
            ChangeCharColor(new Color(243f / 255f, 128f / 255f, 128f / 255f));
            isColliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Object"))
        {
            ChangeCharColor(Color.white);
            isColliding = false;
            CheckTile();
        }
    }
}