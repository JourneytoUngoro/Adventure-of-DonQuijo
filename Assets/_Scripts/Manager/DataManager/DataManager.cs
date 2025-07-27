using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;
using System;
using Sirenix.OdinInspector;

public class DataManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool disableAutoSaving = false;
    [SerializeField] private bool initializeDataIfNull = false;
    [SerializeField] private bool overrideSelectedProfileId;
    [SerializeField] private string testSelectedProfileId = "test";

    [Header("File Storage Config")]
    [Tooltip("Full Path \"C:/Users/users/AppData/LocalLow/DefaultCompany/Belt Scroller/nowProfileId/fileName\"")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    public string selectedProfileId { get; private set; } // Profile Id of current save file. Initial value is null when nothing is selected. Value changes when the player selects a save slot.

    public GameData gameData { get; private set; } // current selected Profile Id's game data

    [ShowInInspector]
    private List<IDataPersistance> dataPersistanceObjects;
    private FileDataHandler dataHandler;

    [SerializeField] private List<SceneField> excludedScenesForData; // 

    [SerializeField] private float autoSaveTimeSeconds = 60f;
    Coroutine AutoSaveCoroutine = null;

    private void Awake()
    {
        if (disableAutoSaving)
        {
            Debug.LogWarning("Auto saving is currently disabled. No auto save supported when you leave the game.");
        }

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);

        InitializeSelectedProfileId();

        bool autoSaveValue = PlayerPrefs.GetInt(EnvironmentController.AUTO_SAVE, 1) == 1;
        SetAutoSaveDisable(!autoSaveValue);
    }
    private void OnDestroy()
    {
        Debug.Log($"DataManager : Destroyed at scene: {SceneManager.GetActiveScene().name}");
    }

    private void Start()
    {
        InvokeRepeating("SaveGame", 60.0f, 60.0f);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (IsExcludedScene()) return;

        this.dataPersistanceObjects = FindAllDataPersistenceObjects();
        LoadGame();

        if (!disableAutoSaving) { AutoSaveCoroutine = StartCoroutine(AutoSave()); }
    }

    public void ChangeSelectedProfileId(string newProfileId)
    {
        this.selectedProfileId = newProfileId;
        LoadGame();
    }

    public void DeleteProfileData(string profileId)
    {
        dataHandler.Delete(profileId);
        InitializeSelectedProfileId();
        LoadGame();
    }

    private void InitializeSelectedProfileId()
    {
        this.selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();

        if (overrideSelectedProfileId)
        {
            selectedProfileId = testSelectedProfileId;
            Debug.LogWarning("Overrode selected profile type with test type: " + testSelectedProfileId);
        }
    }

    public void NewGame()
    {
        gameData = new GameData();
        Debug.Log("New data created");
    }

    public bool SaveGame()
    {
        if (IsExcludedScene())
        {
            Debug.Log("Excluded Scene");
            return false;
        }

        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found. A new game needs to be started before data can be saved.");
            return false;
        }

        foreach (IDataPersistance dataPersistanceObject in dataPersistanceObjects)
        {
            dataPersistanceObject.SaveData(gameData);
        }

        gameData.displayedLastPlayTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        gameData.lastPlayTime = DateTime.Now.Ticks;

        bool saved = dataHandler.Save(gameData, selectedProfileId);
        return saved;
    }

    public void LoadGame()
    {
        if (IsExcludedScene())
        {
            Debug.Log("Excluded Scene");
            return;
        }

        this.gameData = dataHandler.Load(selectedProfileId);

        Debug.Log($"selected profile id : {selectedProfileId}");

        if (this.gameData == null && initializeDataIfNull)
        {
            NewGame();
        }

        if (this.gameData == null)
        {
            Debug.Log("No Data found. A new game has to be started.");
            return;
        }

        foreach (IDataPersistance dataPersistanceObject in dataPersistanceObjects)
        {
            dataPersistanceObject.LoadData(gameData);
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistance> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistance> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IDataPersistance>();

        return new List<IDataPersistance>(dataPersistenceObjects);
    }

    public bool HasGameData()
    {
        return dataHandler.LoadAllProfiles().Count > 0;
    }

    public bool HasGameData(string profileId)
    {
        return dataHandler.Load(profileId) != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }

    public int AllProfilesCount() => dataHandler.AllProfilesCount();

    public bool IsExcludedScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        return excludedScenesForData.Any(sceneField => sceneField.SceneName == currentScene);
    }

    public void SetAutoSaveDisable(bool disable)
    {
        disableAutoSaving = disable;
        Debug.Log(disableAutoSaving ? "disable auto save" : "enable auto save");


        if (AutoSaveCoroutine != null)
        {
            StopCoroutine(AutoSaveCoroutine);
            AutoSaveCoroutine = null;
        }

        if (!disableAutoSaving) // auto-save enabled
        {
            AutoSaveCoroutine = StartCoroutine(AutoSave());
        }
    }

    private IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveTimeSeconds);

            SaveGame();
            Debug.Log("Auto Saved");
        }
    }
}
