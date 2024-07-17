using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] CanvasGroup background;
    [SerializeField] GameObject lightEfx;
    [SerializeField] Button startButton;

    void Start()
    {
        background.alpha = 0f;
        lightEfx.LeanRotateAround(Vector3.forward, 360, 15f).setLoopClamp();
        startButton.interactable = false;
        StartCoroutine(ActiveEfx());
    }

    IEnumerator ActiveEfx()
    {
        yield return new WaitForSeconds(1.5f);
        TurnBackGround();
        startButton.interactable = true;
    }

    public void TurnBackGround()
    {
        background.alpha = 0f;
        background.LeanAlpha(1f, 0.5f);
    }
}
