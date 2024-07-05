using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] CanvasGroup background;
    [SerializeField] GameObject frame;
    [SerializeField] Button closeBtn;
    private void OnEnable()
    {
        FrameEfx();
    }
    // Start is called before the first frame update
    void Start()
    {
        CloseButtonClicked();
    }

    void FrameEfx()
    {
        frame.LeanScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f).setOnComplete(() =>
        {
            frame.LeanScale(Vector3.one, 0.1f);
        });
    }

    void CloseButtonClicked()
    {
        closeBtn.onClick.AddListener(() =>
        {
            background.LeanAlpha(0f, 0.2f).setOnComplete(() =>
            {
                background.alpha = 1f;
                gameObject.SetActive(false);
            });
        });
    }
}
