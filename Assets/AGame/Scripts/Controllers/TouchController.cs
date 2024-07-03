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
    public float rotationSpeed = 5f;
    public float raycastDistance = 0.5f;
    public float mapBoundaryY = 10f;
    public bool isDragging = false;
    public bool isColliding = false;
    public Tile currentBodyTile = null;
    public Tile currentHeadTile = null;
    public Vector2 direction;

    private Collider2D charCollider;
    private Vector2 initialMousePos;
    private Vector2 currentMousePos;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animController = GetComponent<AnimController>();
        charCollider = GetComponent<Collider2D>();
        direction = (headPosition.position - bodyPosition.position).normalized;
    }

    private void OnMouseDown()
    {
        initialMousePos = Input.mousePosition;
        isColliding = false;
        GameObject clickedCharacter = GetCharacterUnderMouse(initialMousePos);

        if (clickedCharacter != null && clickedCharacter.CompareTag("Object"))
        {
            TouchController clickedController = clickedCharacter.GetComponent<TouchController>();
            clickedCharacter.tag = "Player";

            if (currentActivePlayer == null && currentActivePlayer == clickedController)
            {
                currentActivePlayer.ChangeCharColor(Color.white);
                currentActivePlayer.DropObject();
            }
            currentActivePlayer = clickedController;
            currentActivePlayer.TouchObject();
        }
    }

    private void OnMouseDrag()
    {
        if (currentActivePlayer != null)
        {
            currentMousePos = Input.mousePosition;
            float distance = Vector2.Distance(initialMousePos, currentMousePos);

            if (distance > 3f)
            {
                DragObject();
                UpdateHeadAndBodyPositions();
            }
        }
    }

    private void OnMouseUp()
    {
        if (currentActivePlayer != null)
        {
            currentActivePlayer.DropObject();
            if (!isColliding)
            {
                currentActivePlayer.EscapingMovement();
            }
            else
            {
                currentActivePlayer.ResetPositionToSavedTile();
            }
            currentActivePlayer.gameObject.tag = "Object";
            currentActivePlayer = null;
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
        ChangeCharColor(new Color(1f, 0.91f, 0.73f));
        animController.ChangeAnim("idle");
        AudioManager.Instance.PlaySFX("Touch");
    }

    public void DragObject()
    {
        if (!isColliding)
        {
            Vector2 direction = Camera.main.ScreenToWorldPoint(currentMousePos) - bodyPosition.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            bodyPosition.rotation = Quaternion.RotateTowards(bodyPosition.rotation, rotation, rotationSpeed);

            animController.ChangeAnim("move");
            AudioManager.Instance.PlaySFX("Move");
        }
    }

    public void DropObject()
    {
        isDragging = false;
        ChangeCharColor(Color.white);
    }

    public void ResetPositionToSavedTile()
    {
        if (currentHeadTile != null)
        {
            headPosition = currentHeadTile.transform;

            Vector3 upVector = (currentHeadTile.transform.position - transform.position).normalized;
            transform.up = upVector;
        }
    }


    private void UpdateHeadAndBodyPositions()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(headPosition.position, raycastDistance);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Tile"))
            {
                Tile tile = hitCollider.GetComponent<Tile>();
                if (tile != null)
                {
                    currentHeadTile = tile;
                }
            }
        }
    }


    public void EscapingMovement()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(bodyPosition.position, 0.25f, direction, Mathf.Infinity, LayerMask.GetMask("Player"));

        bool objectHit = false;

        if (hits.Length > 0)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.CompareTag("Object"))
                {
                    ChangeCharColor(hit.collider);
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
        if (collision.CompareTag("Tile"))
        {
            Tile tile = collision.GetComponent<Tile>();
            if (tile != null)
            {
                if (Vector3.Distance(bodyPosition.position, tile.transform.position) < raycastDistance)
                {
                    currentBodyTile = tile;
                }
                else if (Vector3.Distance(headPosition.position, tile.transform.position) < raycastDistance)
                {
                    currentHeadTile = tile;
                }
            }
        }

        if (collision.CompareTag("Object"))
        {
            ChangeCharColor(new Color(243f / 255f, 128f / 255f, 128f / 255f));
            isColliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Tile"))
        {
            Tile tile = collision.GetComponent<Tile>();
            if (tile != null)
            {
                if (currentBodyTile == tile)
                {
                    currentBodyTile = null;
                }
                if (currentHeadTile == tile)
                {
                    currentHeadTile = null;
                }
            }
        }

        if (collision.CompareTag("Object"))
        {
            ChangeCharColor(Color.white);
        }
    }
}
