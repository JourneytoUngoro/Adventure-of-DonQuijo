using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class DataManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool disableAutoSaving = false;
    [SerializeField] private bool initializeDataIfNull = false;
    [SerializeField] private bool overrideSelectedProfileId;
    [SerializeField] private string testSelectedProfileId = "test";

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    //============================== 삭제
    public int count;

    public string selectedProfileId { get; private set; } // Profile Id of current save file. Initial value is null when nothing is selected. Value changes when the player selects a save slot.

    public GameData gameData { get; private set; } // current selected Profile Id's game data
    private List<IDataPersistance> dataPersistanceObjects;
    private FileDataHandler dataHandler;

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
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

/*    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }*/

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu") return;

        this.dataPersistanceObjects = FindAllDataPersistenceObjects();
        LoadGame();
        
        // ================================================================= ( 인스펙터 확인 용 )
        count = dataPersistanceObjects.Count;

        // AutoSaveCoroutine = StartCoroutine(AutoSave());
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

    public void SaveGame()
    {
        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found. A new game needs to be started before data can be saved.");
            return;
        }

        foreach (IDataPersistance dataPersistanceObject in dataPersistanceObjects)
        {
            dataPersistanceObject.SaveData(gameData);
        }

        gameData.displayedLastPlayTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        gameData.lastPlayTime = DateTime.Now.Ticks;

        dataHandler.Save(gameData, selectedProfileId);
    }

    public void LoadGame()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu") return;

        this.gameData = dataHandler.Load(selectedProfileId);

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
