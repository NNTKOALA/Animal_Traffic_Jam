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
    public int selectedLevel;
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
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 0);
        PauseGame();
    }

    private void OnApplicationQuit()
    {
        SaveCurrentLevel();
    }

    public void StartNewGame()
    {
        isPlaying = true;
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 0);
        ResumeGame();
        LoadLevel(currentLevel);
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

        if (selectedLevel < currentLevel)
        {
            LoadLevel(selectedLevel + 1);
            UpdateChoosenLevelText(selectedLevel + 1);
        }
        else
        {
            currentLevel = ++currentLevel % mainLevelPrefab.Count;

            int maxLevel = PlayerPrefs.GetInt(PREF_MAX_LEVEL, 0);
            if (currentLevel > maxLevel)
            {
                PlayerPrefs.SetInt(PREF_MAX_LEVEL, currentLevel);
            }
            LoadLevel(currentLevel);
            SaveCurrentLevel();
            UpdateLevelText();
        }
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

    public void SpawnCurrentLevel(int currentLevel)
    {
        LoadLevel(currentLevel);
        SaveCurrentLevel();
        UpdateLevelText();
    }

    public void SpawnLevelById(int id)
    {
        selectedLevel = id;
        LoadLevel(id);
        UpdateChoosenLevelText(id);
    }

    public void DelaySpawnChoosenLevel(int id)
    {
        StartCoroutine(DelayChooseLevel(id));
    }

    IEnumerator DelayChooseLevel(int id)
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

    public void UpdateChoosenLevelText(int selectedLevel)
    {
        if (levelText != null)
        {
            levelText.text = $"Level {selectedLevel + 1}";
        }
        else
        {
            Debug.LogError("Level text is not assigned.");
        }
    }
}
