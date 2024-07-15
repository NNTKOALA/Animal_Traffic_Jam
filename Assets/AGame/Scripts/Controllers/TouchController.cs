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
    Vector2 initialMousePos;
    Vector2 currentMousePos;
    float headValue = 0.48f;
    float bodyValue = 0.5f;
    float moveSpeed = 5f;
    float rotationSpeed = 15f;
    float mapBoundaryY = 10f;

    private Color originalColor;
    private Quaternion originalRotation;

    private void Start()
    {
        animController = GetComponent<AnimController>();
        charCollider = GetComponent<Collider2D>();
        if (partsToColor.Length > 0)
        {
            originalColor = partsToColor[0].color;
        }
    }

    void OnMouseDown()
    {
        Debug.Log("OnMouseDown => " + gameObject.name);
        initialMousePos = Input.mousePosition;
        isDragging = false;
        isColliding = false;
        GameObject clickedCharacter = this.gameObject;

        if (clickedCharacter != null && clickedCharacter.CompareTag("Object"))
        {
            TouchController clickedController = clickedCharacter.GetComponent<TouchController>();
            clickedCharacter.tag = "Player";
            currentActivePlayer = clickedController;
            currentActivePlayer.TouchObject();
        }
    }

    void OnMouseDrag()
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

    void OnMouseUp()
    {
        if (currentActivePlayer != null)
        {
            currentActivePlayer.DropObject();

            if (!isDragging)
            {
                transform.rotation = originalRotation;
                
            }

            if (!isColliding)
            {
                SetPostionToTile();
            }
            else
            {
                animController.ChangeAnim("idle");
                currentActivePlayer.ResetPositionToSavedTile();
            }

            currentActivePlayer.gameObject.tag = "Object";
            currentActivePlayer = null;
            isColliding = false;
        }
    }

    public void TouchObject()
    {
        originalRotation = transform.rotation;
        ChangeCharColor(new Color(1f, 0.91f, 0.73f));
        animController.ChangeAnim("idle");
        AudioManager.Instance.PlaySFX("Touch");
    }

    public void DragObject()
    {
        Vector3 endpoint = Camera.main.ScreenToWorldPoint(currentMousePos);
        Vector3 direction = endpoint - bodyPosition.position;
        Quaternion desiredRotation = Quaternion.LookRotation(Vector3.forward, direction);

        if (!isColliding)
        {
            desiredRotation = Quaternion.Euler(0, 0, desiredRotation.eulerAngles.z);

            bodyPosition.rotation = Quaternion.RotateTowards(bodyPosition.rotation, desiredRotation, rotationSpeed);
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
            EscapingMovement();
        }
    }

    public void SetPostionToTile()
    {
        if (currentHeadTile != null)
        {
            headPosition = currentHeadTile.transform;
            Vector3 upVector = (currentHeadTile.transform.position - transform.position).normalized;
            transform.up = upVector;
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
        Debug.Log("Starting movement outside map.");

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tile"))
        {
            Tile tile = Cache.GetTile(collision);
            if (tile != null && tile.isOccupied)
            {

                ChangeCharColor(new Color(243f / 255f, 128f / 255f, 128f / 255f));
                tile.ChangeToHitColor();
                isColliding = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Tile"))
        {
            Tile tile = Cache.GetTile(collision);
            if (tile != null && tile.isOccupied)
            {
                if (Vector3.Distance(bodyPosition.position, tile.transform.position) < bodyValue)
                {
                    currentBodyTile = tile;
                }
                if (Vector3.Distance(defaultHeadPosition.transform.position, tile.transform.position) < headValue && !isColliding)
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
        }
    }
}
