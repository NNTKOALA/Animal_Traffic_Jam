using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour
{
    public Vector2 t_position;
    public SpriteRenderer spriteRenderer;
    public Vector3 customCoordinate;
    public bool isOccupied = false;

    void Start()
    {
        t_position = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Object"))
        {
            spriteRenderer.color = Color.gray;
            isOccupied = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Object"))
        {
            spriteRenderer.color = Color.white;
            isOccupied = false;
        }
    }
}