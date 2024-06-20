using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class TouchController : MonoBehaviour
{
    public Transform head;
    public SpriteRenderer[] partsToColor;

    private bool isDragging = false;
    private Transform m_transform;

    void Start()
    {
        m_transform = transform;
    }

    void Update()
    {
        CheckMouseInput();
    }

    public void CheckMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Vector2.Distance(mousePosition, head.position) < 0.4f)
            {
                isDragging = true;
                //Character.Instance.OnEscape();
                ChangeColor(new Color(1f, 0.91f, 0.73f));
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            CheckTile();
            ChangeColor(Color.white) ;
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            DragCharacter();
            //Debug.Log("Character is moving");
        }
    }

    public void DragCharacter()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - m_transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        m_transform.rotation = rotation;
    }

    public void CheckTile()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Vector2.Distance(mousePosition, head.position) < 0.4f && isDragging == false)
        {
            LayerMask tileLayerMask = LayerMask.GetMask("Tile");

            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, tileLayerMask);

            if (hit.collider != null && hit.collider.CompareTag("Tile"))
            {
                Vector3 targetPosition = hit.collider.transform.position;
                Vector3 upVector = (targetPosition - transform.position).normalized;

                transform.up = upVector;
            }
            else
            {
                transform.up = Vector3.up;
            }
        }
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
        if (collision.gameObject.CompareTag("Player"))
        {
            ChangeColor(new Color(243f / 255f, 128f / 255f, 128f / 255f));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ChangeColor(Color.white);
        }
    }
}