using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LevelButton : MonoBehaviour
{
    [SerializeField] Button btn;
    [SerializeField] GameObject buttonLock;
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
        buttonLock.SetActive(false);
        buttonIndex.text = (index + 1).ToString();
        indexLevel = index;
    }

    public void CloseButton()
    {
        selectable = false;
        buttonLock.SetActive(true);
    }

    private void OnPress()
    {
        if (selectable)
        {
            onPress?.Invoke(indexLevel);
        }
    }
}
