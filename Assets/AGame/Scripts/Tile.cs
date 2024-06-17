using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Vector2Int gridPosition;
    public bool isOccupied;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(Vector2Int gridPos)
    {
        gridPosition = gridPos;
        isOccupied = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            spriteRenderer.color = Color.gray;
            isOccupied = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            spriteRenderer.color = Color.white;
            isOccupied = false;
        }
    }
}

