using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

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

    [SerializeField] MainMenuUI mainMenuUI;
    [SerializeField] GameObject inGameUI;
    [SerializeField] GameObject winUI;
    [SerializeField] LoadingUI loadingGameUI;
    [SerializeField] Button nextLevelBtn;
    [SerializeField] Button startBtn;

    [SerializeField] ParticleSystem firework1;
    [SerializeField] ParticleSystem firework2;

    // Start is called before the first frame update
    void Start()
    {
        ButtonClicked();
        LoadingGameSceneCoroutine(4f);
        firework1.Stop();
        firework2.Stop();
        mainMenuUI.gameObject.SetActive(true);
        nextLevelBtn.gameObject.SetActive(false);
    }

    private void ButtonClicked()
    {
        startBtn.onClick.AddListener(() =>
        {
            mainMenuUI.gameObject.SetActive(false);
            StartCoroutine(DelayStartGame());
        });
    }

    IEnumerator DelayStartGame()
    {
        yield return new WaitForSeconds(4f);
        GameManager.Instance.SpawnLevelById(GameManager.Instance.currentLevel);
    }

    public void DeactiveAll()
    {
        mainMenuUI.gameObject.SetActive(false);
        inGameUI.SetActive(false);
        winUI.SetActive(false);
        loadingGameUI.gameObject.SetActive(false);
    }

    public void DeActiveInGameUI()
    {
        inGameUI.SetActive(false);
    }

    public void SwitchTo(GameObject ui)
    {
        DeactiveAll();
        ui.gameObject.SetActive(true);
    }

    public void SwitchToMainMenuUI()
    {
        DeactiveAll();
        mainMenuUI.gameObject.SetActive(true);
    }

    public void SwitchToInGameUI()
    {
        SwitchTo(inGameUI);
        mainMenuUI.gameObject.SetActive(false);
    }

    public void SwitchToWinUI()
    {
        nextLevelBtn.gameObject.SetActive(false);
        SwitchTo(winUI);
        firework1.Play();
        firework2.Play();
        inGameUI.SetActive(true);
        Invoke("ActiveButton", 3f);
    }

    public void LoadingGameSceneCoroutine(float delay)
    {
        StartCoroutine(LoadingRoutine(delay));
    }

    private IEnumerator LoadingRoutine(float delay)
    {
        loadingGameUI.gameObject.SetActive(true);
        loadingGameUI.ActiveLoading();
        yield return new WaitForSeconds(delay);
        loadingGameUI.gameObject.SetActive(false);
    }

    public void ActiveButton()
    {
        nextLevelBtn.gameObject.SetActive(true);
    }
}
