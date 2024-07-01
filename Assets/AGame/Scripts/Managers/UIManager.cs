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

    [SerializeField] GameObject mainMenuUI;
    [SerializeField] GameObject inGameUI;
    [SerializeField] GameObject winUI;
    [SerializeField] GameObject loadingGameUI;
    [SerializeField] Button nextLevelBtn;

    [SerializeField] ParticleSystem firework1;
    [SerializeField] ParticleSystem firework2;

    // Start is called before the first frame update
    void Start()
    {
        LoadingGameSceneCoroutine(3f);
        firework1.Stop();
        firework2.Stop();
        mainMenuUI.SetActive(true);
        nextLevelBtn.gameObject.SetActive(false);
    }

    public void DeactiveAll()
    {
        mainMenuUI.SetActive(false);
        inGameUI.SetActive(false);
        winUI.SetActive(false);
        loadingGameUI.SetActive(false);
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
        loadingGameUI.SetActive(true);

        yield return new WaitForSeconds(delay);

        loadingGameUI.SetActive(false);
    }

    public void ActiveButton()
    {
        nextLevelBtn.gameObject.SetActive(true);
    }
}
