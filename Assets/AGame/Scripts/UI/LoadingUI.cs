using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] GameObject efx;

    public void ActiveLoading()
    {
        efx.transform.localPosition = new Vector2(-1563f, -30f);
        ActiveEfx();
    }

    void ActiveEfx()
    {
        efx.LeanMoveLocalX(1564f, 3f).setOnComplete(() =>
        {
            efx.transform.localPosition = new Vector2(-1563f, -30f);

        });
    }
}
