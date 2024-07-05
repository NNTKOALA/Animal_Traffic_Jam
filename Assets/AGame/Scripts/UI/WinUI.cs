using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinUI : MonoBehaviour
{
    [SerializeField] Image title;
    [SerializeField] Button nextLevelBtn;
    private void OnEnable()
    {
        ActiveEfx();
    }

    private void Start()
    {
        NextLevelOnClick();
    }

    void ActiveEfx()
    {
        Color color = title.color;
        color.a = 0;
        title.transform.localScale = Vector3.one * 4f;
        title.gameObject.LeanAlpha(1f, 0.1f).setOnComplete(() =>
        {
            title.transform.LeanScale(Vector3.one * 2.8f, 0.3f).setEaseOutBounce();
        });
    }

    void NextLevelOnClick()
    {
        nextLevelBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.DelaySpawnNextLevel();
            gameObject.SetActive(false);
        });
    }
}
