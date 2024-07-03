using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Color defaultColor = new Color(136 / 255f, 221 / 255f, 131 / 255f, 1f);
    public Color occupiedColor = new Color(65 / 255f, 97 / 255f, 64 / 255f, 1f);
    public Color hitColor = new Color(243f / 255f, 128f / 255f, 128f / 255f);

    public bool isOccupied = false;
    public GameObject _char = null;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Object"))
        {
            if (!isOccupied)
            {
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
            if (isOccupied && _char == collision.gameObject)
            {
                spriteRenderer.color = defaultColor;
                isOccupied = false;
                _char = null;
            }
            else
            {
                spriteRenderer.color = occupiedColor;
                isOccupied = true;
                _char = collision.gameObject;
            }
        }
    }
}