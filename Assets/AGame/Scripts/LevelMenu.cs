using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public List<Button> levelButtonList;

    // Start is called before the first frame update
    void Start()
    {
/*        int maxLevel = PlayerPrefs.GetInt(GameManager.PREF_MAX_LEVEL, 0);

        for (int i = 0; i < levelButtonList.Count; i++)
        {
            if (i <= maxLevel)
            {
                levelButtonList[i].interactable = true;
            }
            else
            {
                levelButtonList[i].interactable = false;
            }
        }*/
    }

    public void SelectLevel(int id)
    {
        UIManager.Instance.SwitchToInGameUI();
        GameManager.Instance.SpawnLevelById(id);
/*        GameManager.Instance.ResetSavePoint();
        GameManager.Instance.StartNewGame();*/

        gameObject.SetActive(false);
    }
}
