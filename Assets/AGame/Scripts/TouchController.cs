using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    public SpriteRenderer[] partsToColor;
    public Transform head;
    public delegate void DargEndedDelegate(Transform transform);
    public DargEndedDelegate dargEndedDelegate;

    private bool isDragging = false;
    private Transform m_transform;

    void Start()
    {
        m_transform = this.transform;
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

            if (Vector2.Distance(mousePosition, head.position) < 0.5f)
            {
                isDragging = true;
                ChangeColor(new Color(1f, 0.91f, 0.73f));
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            dargEndedDelegate(this.transform);
            ChangeColor(Color.white);
        }

        if (isDragging)
        {
            RotateCharacter();
        }
    }

    public void RotateCharacter()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - m_transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        m_transform.rotation = rotation;
    }

    public void ChangeColor(Color newColor)
    {
        foreach (SpriteRenderer part in partsToColor)
        {
            part.color = newColor;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ChangeColor(new Color(243f / 255f, 128f / 255f, 128f / 255f));
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ChangeColor(Color.white);
        }
    }
}
