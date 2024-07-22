using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    AnimController animController;
    public Transform defaultHeadPosition;
    public Transform headPosition;
    public Transform bodyPosition;
    public SpriteRenderer[] partsToColor;
    public static TouchController currentActivePlayer = null;

    public bool isDragging = false;
    public bool isColliding = false;
    public Tile currentBodyTile = null;
    public Tile currentHeadTile = null;

    Collider2D charCollider;
    Vector3 initialMousePos;
    Vector3 currentMousePos;
    Vector3 startDirection = Vector3.zero;
    Vector3 endDirection = Vector3.zero;
    float headValue = 0.5f;
    float bodyValue = 0.5f;
    float moveSpeed = 5f;
    float mapBoundaryY = 10f;
    float maxRotationAngle = 15f;
    float blockedRotationZ;
    bool blockedForward = false;
    bool canForceUpdateRotation = false;
    bool isDraggingForward = false;
    bool isCollidingWithOccupiedTile = false;

    private Color originalColor;

    private void Start()
    {
        animController = GetComponent<AnimController>();
        charCollider = GetComponent<Collider2D>();
        if (partsToColor.Length > 0)
        {
            originalColor = partsToColor[0].color;
        }
    }

    private void OnMouseDown()
    {
        initialMousePos = Input.mousePosition;
        isDragging = false;
        isColliding = false;

        if (gameObject.CompareTag("Object"))
        {
            gameObject.tag = "Player";
            currentActivePlayer = this;
            TouchObject();
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
                isDragging = true;
                DragObject();
            }
        }
    }

    private void OnMouseUp()
    {
        if (currentActivePlayer != null)
        {
            DropObject();
            if (!isColliding)
            {
                SetPositionToTile();
            }
            else
            {
                animController.ChangeAnim("idle");
                ResetPositionToSavedTile();
            }

            ChangeCharColor(Color.white);
            gameObject.tag = "Object";
            currentActivePlayer = null;
            isColliding = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tile"))
        {
            Tile tile = Cache.GetTile(collision);
            if (tile != null && tile.isOccupied)
            {
                blockedRotationZ = bodyPosition.eulerAngles.z;
                blockedForward = isDraggingForward;

                Debug.Log($"Blocked Rotation Z: {blockedRotationZ}");
                ChangeCharColor(new Color(243f / 255f, 128f / 255f, 128f / 255f));
                tile.ChangeToHitColor();
                isColliding = true;
                isCollidingWithOccupiedTile = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Tile") && !isCollidingWithOccupiedTile)
        {
            Tile tile = Cache.GetTile(collision);
            if (tile != null && tile.isOccupied)
            {
                if (Vector3.Distance(bodyPosition.position, tile.transform.position) < bodyValue)
                {
                    currentBodyTile = tile;
                }

                if (Vector3.Distance(defaultHeadPosition.transform.position, tile.transform.position) < headValue)
                {
                    currentHeadTile = tile;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Tile"))
        {
            ChangeCharColor(originalColor);
            isColliding = false;
            isCollidingWithOccupiedTile = false;
        }
    }

    public void TouchObject()
    {
        startDirection = Camera.main.ScreenToWorldPoint(initialMousePos) - bodyPosition.position;
        ChangeCharColor(new Color(1f, 0.91f, 0.73f));
        animController.ChangeAnim("idle");
        AudioManager.Instance.PlaySFX("Touch");
    }

    public void DragObject()
    {
        endDirection = (Vector2)(Camera.main.ScreenToWorldPoint(currentMousePos) - bodyPosition.position);

        float angle = Vector2.SignedAngle(startDirection, endDirection);
        Debug.LogWarning("Angle => " + angle);

        isDraggingForward = angle < 0;

        if (blockedForward)
        {
            canForceUpdateRotation = angle > 0;
        }
        else
        {
            canForceUpdateRotation = angle < 0;
        }

        if (!isColliding || canForceUpdateRotation)
        {
            float clampedAngle = Mathf.Clamp(angle, -maxRotationAngle, maxRotationAngle);
            Quaternion limitedRotation = Quaternion.Euler(0, 0, bodyPosition.eulerAngles.z + clampedAngle);
            bodyPosition.rotation = limitedRotation;
            animController.ChangeAnim("move");
            AudioManager.Instance.PlaySFX("Move");
            startDirection = endDirection;
        }
    }

    public void DropObject()
    {
        isDragging = false;
    }

    public void ResetPositionToSavedTile()
    {
        if (currentHeadTile != null)
        {
            headPosition = currentHeadTile.transform;
            Vector3 direction = (currentHeadTile.transform.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, direction);
                transform.rotation = lookRotation;
            }
        }
    }

    public void SetPositionToTile()
    {
        if (currentHeadTile != null)
        {
            headPosition = currentHeadTile.transform;
            Vector3 direction = (currentHeadTile.transform.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, direction);
                transform.rotation = lookRotation;
            }
            EscapingMovement();
        }
    }

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
                    StartCoroutine(ChangeColorForSeconds(hit.collider, .5f));
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
            animController.ChangeAnim("move");
            AudioManager.Instance.PlaySFX("Move");
            Destroy(gameObject, 2f);
            GameManager.Instance.DecreaseObjectCount();
        }
    }

    private IEnumerator ChangeColorForSeconds(Collider2D collider, float duration)
    {
        ChangeCharColor(collider);
        yield return new WaitForSeconds(duration);
        ResetColor(collider);
    }

    private void ResetColor(Collider2D collider)
    {
        if (collider != null)
        {
            SpriteRenderer[] spriteRenderers = collider.GetComponentsInChildren<SpriteRenderer>();
            if (spriteRenderers != null)
            {
                foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                {
                    spriteRenderer.color = originalColor;
                }
            }
        }
    }

    private IEnumerator MoveCharacterOutsideMap()
    {
        if (charCollider != null)
        {
            charCollider.enabled = false;
        }

        while (bodyPosition.position.y < mapBoundaryY)
        {
            bodyPosition.position += bodyPosition.up * moveSpeed * Time.deltaTime;
            yield return null;
        }

        Debug.Log("Character has moved outside the map.");
    }

    private void ChangeCharColor(Collider2D collider)
    {
        if (collider != null)
        {
            SpriteRenderer[] spriteRenderers = collider.GetComponentsInChildren<SpriteRenderer>();
            if (spriteRenderers != null)
            {
                foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                {
                    spriteRenderer.color = new Color(243f / 255f, 128f / 255f, 128f / 255f);
                }
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
}