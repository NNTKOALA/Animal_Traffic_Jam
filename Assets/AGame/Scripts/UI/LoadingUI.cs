using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] GameObject efx;

    public void ActiveLoading()
    {
        efx.transform.localPosition = new Vector2(-1563f, -30f);
        StartCoroutine(ActiveEfx());
    }

    IEnumerator ActiveEfx()
    {
        efx.LeanMoveLocalX(-261f, 1f);
        yield return new WaitForSeconds(3f);
        efx.LeanMoveLocalX(1564f, 1f).setOnComplete(() =>
        {
            efx.transform.localPosition = new Vector2(-1563f, -30f);

        });
    }
}
