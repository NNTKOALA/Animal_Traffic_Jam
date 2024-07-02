using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    AnimController animController;
    Rigidbody2D rb;
    public Transform headPosition;
    public Transform bodyPosition;
    public SpriteRenderer[] partsToColor;
    public static TouchController currentActivePlayer = null;
    public float moveSpeed = 5f;
    public float rotationSpeed = 2.5f;
    public float raycastDistance = 0.5f;
    public float mapBoundaryY = 10f;

    private Collider2D charCollider;
    private Vector2 initialMousePos;
    private Vector2 currentMousePos;
    private bool isDragging = false;
    private bool isColliding = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animController = GetComponent<AnimController>();
        charCollider = GetComponent<Collider2D>();
    }

    private void OnMouseDown()
    {
        initialMousePos = Input.mousePosition;

        GameObject clickedCharacter = GetCharacterUnderMouse(Input.mousePosition);

        if (clickedCharacter != null && clickedCharacter.CompareTag("Object"))
        {
            TouchController clickedController = clickedCharacter.GetComponent<TouchController>();
            clickedCharacter.tag = "Player";

            if (TouchController.currentActivePlayer == null && TouchController.currentActivePlayer == clickedController)
            {
                TouchController.currentActivePlayer.ChangeCharColor(Color.white);
                TouchController.currentActivePlayer.DropObject();
            }
            TouchController.currentActivePlayer = clickedController;
            TouchController.currentActivePlayer.TouchObject();
        }
    }

    private void OnMouseDrag()
    {
        if (TouchController.currentActivePlayer != null)
        {
            TouchController.currentActivePlayer.DragObject();
        }
    }

    private void OnMouseUp()
    {
        if (TouchController.currentActivePlayer != null)
        {
            TouchController.currentActivePlayer.DropObject();
            if (!isColliding)
            {
                TouchController.currentActivePlayer.EscapingMovement();
            }
            TouchController.currentActivePlayer.gameObject.tag = "Object";
            TouchController.currentActivePlayer = null;
        }
    }

    public GameObject GetCharacterUnderMouse(Vector2 mousePosition)
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Player"));

        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    public void TouchObject()
    {
        isDragging = true;
        ChangeCharColor(new Color(1f, 0.91f, 0.73f));
        animController.ChangeAnim("idle");
        AudioManager.Instance.PlaySFX("Touch");
    }

    public void DragObject()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - bodyPosition.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        bodyPosition.rotation = Quaternion.RotateTowards(bodyPosition.rotation, rotation, 2f);

        animController.ChangeAnim("move");
        AudioManager.Instance.PlaySFX("Move");
    }

    public void DropObject()
    {
        isDragging = false;
        ChangeCharColor(Color.white);
        CheckTile();
    }

    public void CheckTile()
    {
        LayerMask tileLayerMask = LayerMask.GetMask("Tile");
        RaycastHit2D[] hits = new RaycastHit2D[5];

        hits[0] = Physics2D.Raycast(headPosition.position, Vector2.up, raycastDistance, tileLayerMask);
        hits[1] = Physics2D.Raycast(headPosition.position, Vector2.right, raycastDistance, tileLayerMask);
        hits[2] = Physics2D.Raycast(headPosition.position, Vector2.left, raycastDistance, tileLayerMask);
        hits[3] = Physics2D.Raycast(headPosition.position, Vector2.down, raycastDistance, tileLayerMask);

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Tile"))
            {
                Vector3 targetPosition = hit.collider.transform.position;
                Vector3 upVector = (targetPosition - transform.position).normalized;
                transform.up = upVector;
                return;
            }
            MoveToNearestTile();
        }
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
        Vector2 direction = (headPosition.position - bodyPosition.position);
        RaycastHit2D[] hits = Physics2D.CircleCastAll(bodyPosition.position, 0.25f, direction, Mathf.Infinity, LayerMask.GetMask("Player"));

        bool objectHit = false;

        if (hits.Length > 0)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.CompareTag("Object"))
                {
                    ChangeCharColor(hit.collider);
                    CheckTile();
                    Debug.Log("Hit: " + hit.collider.name + ", Tag: " + hit.collider.tag);
                    objectHit = true;
                    animController.ChangeAnim("hit");
                    AudioManager.Instance.PlaySFX("Hit");
                    break;
                }
            }
        }

        if (!objectHit)
        {
            Debug.Log("No object hit detected");
            isColliding = false;
            StartCoroutine(MoveCharacterOutsideMap());
            Destroy(gameObject, 2f);
            GameManager.Instance.DecreaseObjectCount();
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
        animController.ChangeAnim("move");
        AudioManager.Instance.PlaySFX("Move");
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
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Object"))
        {
            ChangeCharColor(Color.white);
        }
    }
}
