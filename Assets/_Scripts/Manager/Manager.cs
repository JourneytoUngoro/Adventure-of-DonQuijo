using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public static Manager Instance { get; private set; }
    public PlayerInputHandler inputHandler { get; private set; }
    public GameManager gameManager { get; private set; }
    public ObjectPoolingManager objectPoolingManager { get; private set; }
    public InventoryManager itemManager { get; private set; }
    public UIManager uiManager { get; private set; }
    public DataManager dataManager { get; private set; }
/*    public SoundFXManager soundFXManager { get; private set; }
*/    public SoundManager soundManager { get; private set; }
    public QuestManager questManager {  get; private set; }
    public StageManager stageManager { get; private set; }
    public SceneTransitionManager sceneTransitionManager { get; private set; }

    private void Awake()
    {
        #region Singleton
        // Instance = this;
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        #endregion

        inputHandler = GetComponentInChildren<PlayerInputHandler>();
        gameManager = GetComponentInChildren<GameManager>();
        objectPoolingManager = GetComponentInChildren<ObjectPoolingManager>();
        itemManager = GetComponentInChildren<InventoryManager>();
        uiManager = GetComponentInChildren<UIManager>();
        dataManager = GetComponentInChildren<DataManager>();

        // soundFXManager = GetComponentInChildren<SoundFXManager>();
        soundManager = GetComponentInChildren<SoundManager>();
        questManager = GetComponentInChildren<QuestManager>();
        stageManager = GetComponentInChildren<StageManager>();
        sceneTransitionManager = GetComponentInChildren<SceneTransitionManager>();
    }
}
