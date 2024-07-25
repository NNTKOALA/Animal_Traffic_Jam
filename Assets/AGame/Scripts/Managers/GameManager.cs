using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public const string PREF_MAX_LEVEL = "max_level";
    public static GameManager Instance { get; private set; }
    public List<LevelPoint> mainLevelPrefab;
    public int currentLevel;
    public int highestUnlockedLevel;
    public int objectCount { get; private set; }
    public TextMeshProUGUI levelText;
    private LevelPoint currentLevelInstance;

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
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 0);
        highestUnlockedLevel = PlayerPrefs.GetInt(PREF_MAX_LEVEL, 0);
    }

    private void OnApplicationQuit()
    {
        SaveCurrentLevel();
    }

    public void OnNewGame()
    {
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance.gameObject);
        }
        UIManager.Instance.SwitchToMainMenuUI();
        SaveCurrentLevel();
    }

    int CountObjectsWithTag(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        return objects.Length;
    }

    public void NextLevel()
    {
        Debug.Log("Next Level");
        currentLevel = (currentLevel + 1) % mainLevelPrefab.Count;
        UpdateUnlockedLevels();
        LoadLevel(currentLevel);
        SaveCurrentLevel();
        UpdateLevelText();
    }

    public void DelaySpawnNextLevel()
    {
        StartCoroutine(DelayNextLevel());
    }

    IEnumerator DelayNextLevel()
    {
        yield return new WaitForSeconds(1.5f);
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance.gameObject);
        }
        UIManager.Instance.SwitchToInGameUI();
        NextLevel();
    }

    public void SpawnLevelById(int id)
    {
        if (id <= highestUnlockedLevel)
        {
            currentLevel = id;
            LoadLevel(id);
            SaveCurrentLevel();
            UpdateLevelText();
        }
        else
        {
            Debug.LogWarning("Attempted to spawn a locked level!");
        }
    }

    public void DelaySpawnLevel(int id)
    {
        StartCoroutine(DelayLevel(id));
    }

    IEnumerator DelayLevel(int id)
    {
        yield return new WaitForSeconds(1.5f);
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance.gameObject);
        }
        UIManager.Instance.SwitchToInGameUI();
        SpawnLevelById(id);
    }

    private void LoadLevel(int levelIndex)
    {
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance.gameObject);
        }
        currentLevelInstance = Instantiate(mainLevelPrefab[levelIndex]);
        objectCount = CountObjectsWithTag("Object");
        Debug.LogWarning("Object in level => " + objectCount);
    }

    public void DecreaseObjectCount()
    {
        objectCount--;
        Debug.LogWarning("Current Object in level => " + objectCount);
        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        if (objectCount == 0)
        {
            Debug.Log("Win" + objectCount);
            UIManager.Instance.SwitchToWinUI();
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
            Debug.LogError("Level text is not assigned.");
        }
    }

    public void ButtonClick()
    {
        AudioManager.Instance.PlaySFX("Click");
    }

    private void SaveCurrentLevel()
    {
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.Save();
    }

    private void UpdateUnlockedLevels()
    {
        if (currentLevel > highestUnlockedLevel)
        {
            highestUnlockedLevel = currentLevel;
            PlayerPrefs.SetInt(PREF_MAX_LEVEL, highestUnlockedLevel);
            PlayerPrefs.Save();
        }
    }

    public bool IsLevelUnlocked(int levelId)
    {
        return levelId <= highestUnlockedLevel;
    }
}