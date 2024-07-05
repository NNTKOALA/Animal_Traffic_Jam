using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class Tile : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Color defaultColor = new Color(136 / 255f, 221 / 255f, 131 / 255f, 1f);
    public Color occupiedColor = new Color(65 / 255f, 97 / 255f, 64 / 255f, 1f);
    public Color hitColor = new Color(243f / 255f, 128f / 255f, 128f / 255f);

    public bool isOccupied = false;
    public GameObject _char = null;
    //Vector3 screenPos;
    //Vector2 rayOrigin;

    private void Start()
    {
        //screenPos = GameManager.Instance.mainCamera.WorldToScreenPoint(transform.position);
        //rayOrigin = GameManager.Instance.mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, GameManager.Instance.mainCamera.nearClipPlane));
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Object"))
        {
            if (!isOccupied)
            {
                Debug.Log($"<color=yellow>Character trigger : {collision.name}</color>");
                spriteRenderer.color = occupiedColor;
                isOccupied = true;
                _char = collision.gameObject;
            }
            else if (_char != collision.gameObject)
            {
                spriteRenderer.color = hitColor;
                isOccupied = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Object"))
        {
            TouchController _player = collision.GetComponent<TouchController>();
            if (isOccupied && _char == collision.gameObject)
            {
                Debug.Log($"<color=red> Object exit : {collision.name} </color>");
                spriteRenderer.color = defaultColor;
                isOccupied = false;
                _char = null;
            }
            /*else
            {
                Debug.Log($"<color=yellow> Object exit : {collision.name} </color>");
                spriteRenderer.color = occupiedColor;
                isOccupied = true; 
                _char = collision.gameObject;
            }*/

        }
        //CheckStatusTile();
    }

    void CheckStatusTile()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.3f, -transform.forward);
        if (hit.collider.CompareTag("Object"))
        {
            Debug.Log($"<color=blue>{gameObject.name} Hitter : {hit.collider.name}</color>");
            spriteRenderer.color = occupiedColor;
            isOccupied = true;
            _char = hit.collider.gameObject;
        }
        else
        {
            Debug.Log($"<color=blue>{gameObject.name} No Hitter Object</color>");
            spriteRenderer.color = defaultColor;
            isOccupied = false;
            _char = null;
        }
    }


    public void ChangeToHitColor()
    {
        StartCoroutine(ChangeHitColor());
    }

    IEnumerator ChangeHitColor()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = occupiedColor;
    }
}