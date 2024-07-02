using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    Vector2 difference = Vector2.zero;
    public SpriteRenderer[] partsToColor;

    private void OnMouseDown()
    {
        Vector2 initialMousePos = Input.mousePosition;
        RaycastHit2D hit = Physics2D.Raycast(initialMousePos, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Player"));
        if (hit.collider != null && hit.collider.gameObject.CompareTag("Object"))
        {
            hit.collider.gameObject.tag = "Player";

        }
        difference = (Vector2)Camera.main.ScreenToWorldPoint(initialMousePos) - (Vector2)transform.position;
    }

    private void OnMouseDrag()
    {
        Vector2 direction = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - difference;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void ChangeCharColor(Color newColor)
    {
        foreach (SpriteRenderer part in partsToColor)
        {
            part.color = newColor;
        }
    }
}
