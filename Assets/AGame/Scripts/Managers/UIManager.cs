using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    [SerializeField] GameObject mainMenuUI;
    [SerializeField] GameObject inGameUI;
    [SerializeField] GameObject levelManagerUI;
    [SerializeField] GameObject settingUI;

    // Start is called before the first frame update
    void Start()
    {
        mainMenuUI.SetActive(true);
    }

    public void DeactiveAll()
    {
        mainMenuUI.SetActive(false);
        inGameUI.SetActive(false);
        levelManagerUI.SetActive(false);
        settingUI.SetActive(false);
    }

    public void SwitchTo(GameObject ui)
    {
        DeactiveAll();
        ui.gameObject.SetActive(true);
    }

    public void SwitchToMainMenuUI()
    {
        DeactiveAll();
        mainMenuUI.SetActive(true);
    }

    public void SwitchToInGameUI()
    {
        SwitchTo(inGameUI);
        mainMenuUI.SetActive(false);
    }

    public void SwitchToLevelManagerUI()
    {
        SwitchTo(levelManagerUI);
        mainMenuUI.SetActive(true);
    }

    public void SwitchToSettingUI()
    {
        SwitchTo(settingUI);
    }

    public void DisableSettingUI()
    {
        settingUI.SetActive(false);
    }
}
