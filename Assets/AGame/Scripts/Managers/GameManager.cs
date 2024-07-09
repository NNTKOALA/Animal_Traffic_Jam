using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public const string PREF_MAX_LEVEL = "max_level";

    public static GameManager Instance { get; private set; }

    public List<LevelPoint> mainLevelPrefab;
    public int currentLevel;
    public int objectCount { get; private set; }

    public TextMeshProUGUI levelText;

    private LevelPoint currentLevelInstance;
    private bool isPlaying;

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

    void Start()
    {
        currentLevel = PlayerPrefs.GetInt("current_level");
        PauseGame();
        UpdateLevelText();
    }

    void Update()
    {

    }

    public void StartNewGame()
    {
        currentLevel = PlayerPrefs.GetInt("current_level");
        isPlaying = true;
        LoadLevel(currentLevel);
        ResumeGame();
    }

    public void OnNewGame()
    {
        SaveCurrentLevel();
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance.gameObject);
        }
        UIManager.Instance.SwitchToMainMenuUI();
    }

    public void PauseGame()
    {
        isPlaying = false;
    }

    public void ResumeGame()
    {
        isPlaying = true;
    }

    int CountObjectsWithTag(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        return objects.Length;
    }

    public void NextLevel()
    {
        Debug.Log("Next Level");
        currentLevel = ++currentLevel % mainLevelPrefab.Count;

        int maxLevel = PlayerPrefs.GetInt(PREF_MAX_LEVEL, 0);
        if (currentLevel > maxLevel)
        {
            PlayerPrefs.SetInt(PREF_MAX_LEVEL, currentLevel);
        }

        SaveCurrentLevel();
        LoadLevel(currentLevel);
    }

    public void DelaySpawnNextLevel()
    {
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance.gameObject);
        }
        UIManager.Instance.DeActiveInGameUI();
        StartCoroutine(DelayNextLevel());
    }

    IEnumerator DelayNextLevel()
    {
        yield return new WaitForSeconds(4f);
        UIManager.Instance.SwitchToInGameUI();
        NextLevel();
    }

    public void SpawnLevelById(int id)
    {
        currentLevel = id;
        SaveCurrentLevel();
        LoadLevel(currentLevel);
    }

    private void LoadLevel(int levelIndex)
    {
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance.gameObject);
        }

        currentLevelInstance = Instantiate(mainLevelPrefab[levelIndex]);
        objectCount = CountObjectsWithTag("Object");
        UIManager.Instance.SwitchToInGameUI();
        UpdateLevelText();
    }

    public void DecreaseObjectCount()
    {
        objectCount--;
        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        if (objectCount <= 0)
        {
            Debug.Log("Win" + objectCount);
            UIManager.Instance.SwitchToWinUI();
            PauseGame();
        }
    }

    public void UpdateLevelText()
    {
        if (levelText != null)
        {
            levelText.text = $"Level {currentLevel + 1}";
        }
        else
        {
            levelText.text = $"You finish the beta test";
        }
    }

    public void ButtonClick()
    {
        AudioManager.Instance.PlaySFX("Click");
    }

    private void SaveCurrentLevel()
    {
        PlayerPrefs.SetInt("current_level", currentLevel);
        PlayerPrefs.Save();
    }
}
