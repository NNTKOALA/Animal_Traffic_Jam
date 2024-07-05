using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] CanvasGroup background;
    [SerializeField] GameObject lightEfx;

    void Start()
    {
        
        background.alpha = 0f;
        lightEfx.LeanRotateAround(Vector3.forward, 360, 15f).setLoopClamp();
        StartCoroutine(ActiveEfx());
    }
    IEnumerator ActiveEfx()
    {
        yield return new WaitForSeconds(4f);
        TurnBackGround();
    }

    public void TurnBackGround()
    {
        background.alpha = 0f;
        background.LeanAlpha(1f, 0.5f);
    }
}
