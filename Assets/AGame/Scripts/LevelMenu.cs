using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public List<LevelButton> levelButtonList;
    [SerializeField] CanvasGroup background;
    [SerializeField] GameObject frame;
    [SerializeField] Button closeBtn;

    private void OnEnable()
    {
        FrameEfx();

        int maxLevel = GameManager.Instance.currentLevel;

        for (int i = 0; i < levelButtonList.Count; i++)
        {
            if (i == maxLevel)
            {
                levelButtonList[i].CurrentLevelButton(SelectLevel, i);
            }
            else if (i <= maxLevel)
            {
                levelButtonList[i].OpenButton(SelectLevel, i);
            }
            else
            {
                levelButtonList[i].CloseButton();
            }
        }
        CloseButtonClicked();
    }

    public void SelectLevel(int id)
    {
        Debug.Log("Select Level => " + id);
        UIManager.Instance.LoadingGameSceneCoroutine(3f);
        GameManager.Instance.DelaySpawnChoosenLevel(id);
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
