using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleController : MonoBehaviour
{
    AnimController animController;

    private void Start()
    {
        animController = GetComponent<AnimController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Get Hit");
            GetHit();
            Debug.Log("Turtle hited by: " + collision.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnIdle();
        }
    }

    public void GetHit()
    {
        animController.ChangeAnim("hit");
    }

    public void OnIdle()
    {
        animController.ChangeAnim("idle");
    }
}
