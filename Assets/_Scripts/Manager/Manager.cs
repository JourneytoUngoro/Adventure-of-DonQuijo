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
    public ItemManager itemManager { get; private set; }
    public UIManager uiManager { get; private set; }
    public DataManager dataManager { get; private set; }
/*    public SoundFXManager soundFXManager { get; private set; }
*/    public SoundManager soundManager { get; private set; }

    private void Awake()
    {
        #region Singleton
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
        itemManager = GetComponentInChildren<ItemManager>();
        uiManager = GetComponentInChildren<UIManager>();
        dataManager = GetComponentInChildren<DataManager>();

        // soundFXManager = GetComponentInChildren<SoundFXManager>();
        soundManager = GetComponentInChildren<SoundManager>();
    }

    #region Test

    [SerializeField] private List<SceneField> scenesToInitialize;
    public Player player { get; private set; }

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
        if (IsInitializeScene(scene.name))
        {
            Debug.Log($"{scene.name}");

            FindPlayer();

            // itemManager.SetupInGameScene();
        }
    }

    bool IsInitializeScene(string nextScene)
    {
        return scenesToInitialize.Any(SceneField => SceneField ==  nextScene);
    }

    public void FindPlayer()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        Debug.Log(player == null ? "can't find player" : "found player");
        Debug.Assert(player != null, $"can't find player on the {SceneManager.GetActiveScene().name}!");
    }
    #endregion

}
