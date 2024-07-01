using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public List<LevelButton> levelButtonList;

    // Start is called before the first frame update
    void Start()
    {
        int maxLevel = GameManager.Instance.currentLevel;

        for (int i = 0; i < levelButtonList.Count; i++)
        {
            if (i <= maxLevel)
            {
                levelButtonList[i].OpenButton(SelectLevel, i);
            }
            else
            {
                levelButtonList[i].CloseButton();
            }
        }
    }

    public void SelectLevel(int id)
    {
        GameManager.Instance.SpawnLevelById(id);
    }
}
