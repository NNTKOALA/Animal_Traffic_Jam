using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public const string PREF_MAX_LEVEL = "max_level";

    public static GameManager Instance { get; private set; }

    public List<LevelManager> mainLevelPrefab;
    public int currentLevel;
    private LevelManager currentLevelInstance;
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

    // Start is called before the first frame update
    void Start()
    {
        currentLevel = 0;
        PauseGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WinGame()
    {
        //UIManager.Instance.SwitchToWinPanel();
        PauseGame();
    }

    public void PauseGame()
    {
        isPlaying = false;
    }

    public void ResumeGame()
    {
        isPlaying = true;
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

        Destroy(currentLevelInstance.gameObject);
        currentLevelInstance = Instantiate(mainLevelPrefab[currentLevel]);
        //UIManager.Instance.SwitchToIngameUI();
    }

    public void SpawnLevelById(int id)
    {
        currentLevel = id;
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance.gameObject);
        }
        currentLevelInstance = Instantiate(mainLevelPrefab[currentLevel]);
        //UIManager.Instance.SwitchToIngameUI();
    }
}
