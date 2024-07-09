using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    AnimController animController;
    Rigidbody2D rb;
    public Transform defaultHeadPosition;
    public Transform headPosition;
    public Transform bodyPosition;
    public SpriteRenderer[] partsToColor;
    public static TouchController currentActivePlayer = null;
    public bool isDragging = false;
    public bool isColliding = false;
    public Tile currentBodyTile = null;
    public Tile currentHeadTile = null;
    private Vector3 currentHeadPosition;
    private Vector3 currentBodyPosition;

    Collider2D charCollider;
    Vector2 initialMousePos;
    Vector2 currentMousePos;
    float headValue = 0.4f;
    float bodyValue = 0.5f;
    float moveSpeed = 5f;
    float rotationSpeed = 5f;
    float mapBoundaryY = 10f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animController = GetComponent<AnimController>();
        charCollider = GetComponent<Collider2D>();
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
            //currentActivePlayer.GetTilePosition();
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
                //GetTilePosition();
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
                SetPostionToTile();
                //GetTilePosition();
            }
            else
            {
                currentActivePlayer.ResetPositionToSavedTile();
            }
            currentActivePlayer.gameObject.tag = "Object";
            currentActivePlayer = null;
            isColliding = false;
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
            Vector3 endpoint = Camera.main.ScreenToWorldPoint(currentMousePos);
            Vector3 direction = endpoint - bodyPosition.position;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Vector3 newRotation = new Vector3(0, 0, angle - 90);
            bodyPosition.eulerAngles = Vector3.Lerp(bodyPosition.eulerAngles, newRotation, rotationSpeed);

            animController.ChangeAnim("move");
            AudioManager.Instance.PlaySFX("Move");
        }
    }

    public void DropObject()
    {
        isDragging = false;
        ChangeCharColor(Color.white);
    }

    /*    public void GetTilePosition()
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(headPosition.position, 0.5f, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Tile"));

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.CompareTag("Tile"))
                {
                    Tile tile = Cache.GetTile(hit.collider);
                    //Debug.Log($"<color=green>Head Distance {gameObject.name} and {tile.name} is {Vector3.Distance(defaultHeadPosition.position, tile.transform.position)}</color>");
                    if (!tile.isOccupied && Vector3.Distance(defaultHeadPosition.transform.position, tile.transform.position) < headValue && !isColliding)
                    {
                        //Debug.Log($"<color=red> Head Distance : {Vector3.Distance(defaultHeadPosition.position, tile.transform.position)}</color>");
                        currentHeadTile = tile;
                    }
                }
            }
        }*/

    public void ResetPositionToSavedTile()
    {
        if (currentHeadTile != null)
        {
            headPosition = currentHeadTile.transform;
            Vector3 upVector = (currentHeadTile.transform.position - transform.position).normalized;
            transform.up = upVector;
            EscapingMovement();
        }
    }

    public void SetPostionToTile()
    {
        if (currentHeadTile != null)
        {
            //Debug.Log($"Set head position : {currentHeadTile.name}");
            headPosition = currentHeadTile.transform;
            Vector3 upVector = (currentHeadTile.transform.position - transform.position).normalized;
            transform.up = upVector;
            EscapingMovement();
        }
    }

/*    public void ResetPositionToSavedTile()
    {
        headPosition.position = currentHeadPosition;
        bodyPosition.position = currentBodyPosition;
        Vector3 upVector = (headPosition.position - transform.position).normalized;
        transform.up = upVector;
    }

    public void GetTilePosition()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(headPosition.position, 0.5f, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Tile"));

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Tile"))
            {
                Tile tile = Cache.GetTile(hit.collider);
                if (!tile.isOccupied && Vector3.Distance(defaultHeadPosition.position, tile.transform.position) < headValue && !isColliding)
                {
                    currentHeadTile = tile;
                    currentHeadPosition = tile.transform.position;
                }
            }
        }
    }*/


    public void EscapingMovement()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(bodyPosition.position, 0.25f, bodyPosition.up, Mathf.Infinity, LayerMask.GetMask("Player"));
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
            //bodyPosition.position += (Vector3)direction * moveSpeed * Time.deltaTime;
            bodyPosition.position += bodyPosition.up * moveSpeed * Time.deltaTime;
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
                if (tile.isOccupied)
                {
                    ChangeCharColor(new Color(243f / 255f, 128f / 255f, 128f / 255f));
                    tile.ChangeToHitColor();
                    isColliding = true;
                    return;
                }
            }
        }
    }

    /*    private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Tile"))
            {
                Tile tile = collision.GetComponent<Tile>();
                if (tile != null)
                {
                    if (tile.isOccupied)
                    {
                        ChangeCharColor(new Color(243f / 255f, 128f / 255f, 128f / 255f));
                        tile.ChangeToHitColor();
                        isColliding = true;
                        currentHeadPosition = headPosition.position;
                        currentBodyPosition = bodyPosition.position;
                        return;
                    }
                }
            }
        }*/

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Tile"))
        {
            Tile tile = Cache.GetTile(collision);
            if (Vector3.Distance(bodyPosition.position, tile.transform.position) < bodyValue)
            {
                currentBodyTile = tile;
            }
            //Debug.Log($"<color=green>Head Distance {gameObject.name} and {tile.name} is {Vector3.Distance(defaultHeadPosition.position, tile.transform.position)}</color>");
            if (Vector3.Distance(defaultHeadPosition.transform.position, tile.transform.position) < headValue && !isColliding)
            {
                //Debug.Log($"<color=red> Head Distance : {Vector3.Distance(defaultHeadPosition.position, tile.transform.position)}</color>");
                currentHeadTile = tile;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Object"))
        {
            ChangeCharColor(Color.white);
        }
    }
}