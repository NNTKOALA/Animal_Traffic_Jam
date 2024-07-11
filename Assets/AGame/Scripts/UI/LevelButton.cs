using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LevelButton : MonoBehaviour
{
    [SerializeField] Button btn;
    [SerializeField] Image buttonOpen;
    [SerializeField] Image buttonCurrentLevel;
    [SerializeField] Image buttonLock;
    [SerializeField] TMP_Text buttonIndex;
    private bool selectable = false;
    private int indexLevel;
    private Action<int> onPress;

    void Start()
    {
        btn.onClick.AddListener(OnPress);
    }

    public void OpenButton(Action<int> onButtonPress, int index)
    {
        selectable = true;
        onPress = onButtonPress;
        buttonOpen.enabled = true;
        buttonCurrentLevel.enabled = false;
        buttonLock.enabled = false;
        buttonIndex.text = (index + 1).ToString();
        indexLevel = index;
    }

    public void CurrentLevelButton(Action<int> onButtonPress, int index)
    {
        selectable = true;
        onPress = onButtonPress;
        buttonOpen.enabled = false;
        buttonCurrentLevel.enabled = true;
        buttonLock.enabled = false;
        buttonIndex.text = (index + 1).ToString();
        indexLevel = index;
    }

    public void CloseButton()
    {
        selectable = false;
        buttonOpen.enabled = false;
        buttonCurrentLevel.enabled = false;
        buttonLock.enabled = true;
    }

    private void OnPress()
    {
        if (selectable)
        {
            onPress?.Invoke(indexLevel);
        }
    }
}
