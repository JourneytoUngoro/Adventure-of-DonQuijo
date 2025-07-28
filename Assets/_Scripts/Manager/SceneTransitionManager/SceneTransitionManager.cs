using AYellowpaper.SerializedCollections;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour, IDataPersistance
{
    public bool isFadingIn { get; private set; }
    public bool isFadingOut { get; private set; }

    [SerializeField] private SceneField currentActiveScene;
    [SerializeField] private SerializedDictionary<SceneField, List<SceneField>> adjacentScene;

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [Range(0.1f, 3.0f), SerializeField] private float minDuration;

    [SerializeField] private GameObject statusBar;
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TMP_Text loadingBarText;
    [SerializeField] private List<string> loadingText;

    private Dictionary<string, AsyncOperation> currentAsyncOperationDictionary = new Dictionary<string, AsyncOperation>();
    private DoorTriggerInteraction.DoorToSpawnAt doorToSpawnAt;
    private SceneConnectorInteraction.Direction direction;
    private Transform destinationTransform;
    private float elapsedTime;
    private float currentProgress;
    private bool useLoadingBar;

    private Coroutine fadeInOutCoroutine;
    private Coroutine sceneTransitionCoroutine;

    private void Start()
    {
        currentActiveScene = new SceneField(SceneManager.GetActiveScene().name);
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (loadingBar != null)
        {
            loadingBar.value = Mathf.MoveTowards(loadingBar.value, currentProgress, 3.0F * Time.unscaledDeltaTime);
        }

        /*if (isFadingOut)
        {
            elapsedTime += Time.unscaledDeltaTime;

            if (fadeInOutImage.color.a < 1.0f)
            {
                currentColor.a = Mathf.Clamp(elapsedTime / minDuration, 0.0f, 1.0f);
                fadeInOutImage.color = currentColor;
            }
            else
            {
                loadingBarText.text = "Game Tips | " + loadingText.GetRandom();
                isFadingOut = false;
                elapsedTime = 0.0f;
            }
        }
        else if (isFadingIn)
        {
            elapsedTime += Time.unscaledDeltaTime;

            if (fadeInOutImage.color.a > 0.0f)
            {
                currentColor.a = Mathf.Clamp(1.0f - elapsedTime / fadeInTime, 0.0f, 1.0f);
                fadeInOutImage.color = currentColor;
            }
            else
            {
                isFadingIn = false;
                Manager.Instance.gameManager.ResumeGame();
                if (Manager.Instance.player.gameObject.activeSelf)
                {
                    Manager.Instance.inputHandler.playerInput.currentActionMap.Enable();
                }
            }
        }*/
    }

    // called on scene transition
    private IEnumerator FadeOut(bool useLoadingBar, string targetScene)
    {
        Manager.Instance.gameManager.PauseGame();

        if (Player.Instance.gameObject.activeSelf)
        {
            Manager.Instance.inputHandler.playerInput.currentActionMap.Disable();
        }

        ImageUI fadeInOutImage = Manager.Instance.uiManager.GetUI<ImageUI>(UIType.FadeImage);

        fadeInOutImage.ShowUI(null);

        yield return new WaitForSecondsRealtime(fadeInOutImage.FadeTime());

        if (!IsLoadingScene(targetScene))
        {
            Debug.Log("Scene was not loaded. Start Loading the target scene: " + targetScene);
            currentAsyncOperationDictionary.Add(targetScene, SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive));
        }

        if (useLoadingBar)
        {
            LoadingBar(targetScene);
        }

        yield return new WaitForSecondsRealtime(minDuration);

        loadingBar.gameObject.SetActive(false);

        yield return new WaitUntil(() => SceneManager.GetSceneByName(targetScene).isLoaded);

        fadeInOutImage.HideUI(() => Manager.Instance.gameManager.ResumeGame());
    }

    public async void LoadingBar(string loadingScene)
    {
        if (loadingBar == null)
        {
            Debug.Log("No Loading Bar exists. Return the function.");
            return;
        }

        AsyncOperation asyncOperation;

        loadingBar.value = 0.0f;
        loadingBar.gameObject.SetActive(true);
        loadingBarText.text = "Game Tips | " + loadingText.GetRandom();

        if (!currentAsyncOperationDictionary.TryGetValue(loadingScene, out asyncOperation))
        {
            Debug.Log($"Can't find async operation of {loadingScene}.");
            currentProgress = 1.0f;
        }
        else
        {
            currentProgress = 0.0f;
        }

        do
        {
            currentProgress = asyncOperation.progress;
            await Task.Delay(100);
        } while (loadingBar.value < 1.0f);
    }

    public void SceneTransition(SceneField targetScene, DoorTriggerInteraction.DoorToSpawnAt doorToSpawnAt, bool useFadeInOut = false, bool useLoadingBar = false)
    {
        this.doorToSpawnAt = doorToSpawnAt;
        this.direction = SceneConnectorInteraction.Direction.None;
        this.destinationTransform = null;

        if (sceneTransitionCoroutine != null)
        {
            StopCoroutine(sceneTransitionCoroutine);
        }
        sceneTransitionCoroutine = StartCoroutine(SceneTransitionCoroutine(targetScene, useFadeInOut, useLoadingBar));
    }

    public void SceneTransition(SceneField targetScene, SceneConnectorInteraction.Direction direction, bool useFadeInOut = false, bool useLoadingBar = false)
    {
        this.doorToSpawnAt = DoorTriggerInteraction.DoorToSpawnAt.None;
        this.direction = direction;
        this.destinationTransform = null;

        if (sceneTransitionCoroutine != null)
        {
            StopCoroutine(sceneTransitionCoroutine);
        }
        sceneTransitionCoroutine = StartCoroutine(SceneTransitionCoroutine(targetScene, useFadeInOut, useLoadingBar));
    }

    public void SceneTransition(SceneField targetScene, bool useFadeInOut = false, bool useLoadingBar = false)
    {
        this.doorToSpawnAt = DoorTriggerInteraction.DoorToSpawnAt.None;
        this.direction = SceneConnectorInteraction.Direction.None;
        this.destinationTransform = null;

        if (sceneTransitionCoroutine != null)
        {
            StopCoroutine(sceneTransitionCoroutine);
        }
        sceneTransitionCoroutine = StartCoroutine(SceneTransitionCoroutine(targetScene, useFadeInOut, useLoadingBar));
    }

    public void SceneTransition(SceneField targetScene, Transform destinationTransform, bool useFadeInOut = false, bool useLoadingBar = false)
    {
        this.doorToSpawnAt = DoorTriggerInteraction.DoorToSpawnAt.None;
        this.direction = SceneConnectorInteraction.Direction.None;
        this.destinationTransform = destinationTransform;

        if (sceneTransitionCoroutine != null)
        {
            StopCoroutine(sceneTransitionCoroutine);
        }
        sceneTransitionCoroutine = StartCoroutine(SceneTransitionCoroutine(targetScene, useFadeInOut, useLoadingBar));
    }

    private IEnumerator SceneTransitionCoroutine(SceneField targetScene, bool useFadeInOut = false, bool useLoadingBar = false)
    {
        if (useFadeInOut)
        {
            if (fadeInOutCoroutine != null)
            {
                StopCoroutine(fadeInOutCoroutine);
            }
            yield return StartCoroutine(FadeOut(useLoadingBar, targetScene));
        }
        else if (!IsLoadingScene(targetScene))
        {
            Debug.Log("Scene was not loaded. Start Loading the target scene: " + targetScene);
            currentAsyncOperationDictionary.Add(targetScene, SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive));
        }

        if (!SceneManager.GetSceneByName(targetScene.SceneName).isLoaded)
        {
            Debug.Log($"Target scene \"{targetScene}\" is not done loading.");
            Manager.Instance.gameManager.PauseGame();
            yield return new WaitUntil(() => SceneManager.GetSceneByName(targetScene.SceneName).isLoaded);// && currentAsyncOperationDictionary[targetScene].isDone);
            Manager.Instance.gameManager.ResumeGame();
            Debug.Log($"Target scene \"{targetScene.SceneName}\" is done loading. Set as active scene.");
        }

        currentActiveScene = targetScene;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetScene.SceneName));
    }

    public void SceneTransition(string targetScene, bool useFadeInOut = false, bool useLoadingBar = false)
    {
        this.doorToSpawnAt = DoorTriggerInteraction.DoorToSpawnAt.None;
        this.direction = SceneConnectorInteraction.Direction.None;
        this.destinationTransform = null;

        if (sceneTransitionCoroutine != null)
        {
            StopCoroutine(sceneTransitionCoroutine);
        }
        sceneTransitionCoroutine = StartCoroutine(SceneTransitionCoroutine(new SceneField(targetScene), useFadeInOut, useLoadingBar));
    }

    // called when scene transition is complete
    // load the scenes that are adjacent to current scene and unload that are not
    public void LoadAndUnloadScenes()
    {
        Debug.Log($"Adjacent Scenes of {currentActiveScene.SceneName}: " + string.Join(", ", adjacentScene[currentActiveScene].Select(sceneField => sceneField.SceneName)));
        UnloadScene();
        LoadScene();
    }

    // below function loads the scene that is adjacent to target scene
    private void LoadScene()
    {
        foreach (SceneField sceneField in adjacentScene[currentActiveScene])
        {
            if (!IsLoadingScene(sceneField.SceneName))
            {
                Debug.Log($"Load scene: {sceneField.SceneName}");
                AsyncOperation loadingScene = SceneManager.LoadSceneAsync(sceneField, LoadSceneMode.Additive);
                currentAsyncOperationDictionary.Add(sceneField.SceneName, loadingScene);
            }
        }
    }

    // below function unloads the scenes that seems to be unnecessary
    // wait until fading out is done
    private void UnloadScene()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene currentScene = SceneManager.GetSceneAt(i);

            if (!IsAdjacentScene(currentScene.name) && !currentActiveScene.SceneName.Equals(currentScene.name))
            {
                Debug.Log($"Unload scene: {currentScene.name}");
                SceneManager.UnloadSceneAsync(currentScene);
            }
        }
    }

    private bool IsAdjacentScene(string sceneName)
    {
        foreach (SceneField sceneField in adjacentScene[currentActiveScene])
        {
            if (sceneName.Equals(sceneField.SceneName))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsLoadingScene(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name.Equals(sceneName))
            {
                return true;
            }
        }

        return false;
    }

    private void FindDoor(DoorTriggerInteraction.DoorToSpawnAt doorToSpawnAt)
    {
        if (doorToSpawnAt.Equals(DoorTriggerInteraction.DoorToSpawnAt.None)) return;

        DoorTriggerInteraction[] doorTriggerInteractions = FindObjectsOfType<DoorTriggerInteraction>();

        foreach (DoorTriggerInteraction doorTriggerInteraction in doorTriggerInteractions)
        {
            if (doorTriggerInteraction.currentDoorIndex.Equals(doorToSpawnAt) && (doorTriggerInteraction.gameObject.scene.name == currentActiveScene.SceneName))
            {
                Collider2D doorCollider = doorTriggerInteraction.gameObject.GetComponent<Collider2D>();
                Vector2 groundPosition = new Vector2(doorCollider.bounds.center.x, doorCollider.bounds.min.y);
                Player.Instance.movement.SetPosition(groundPosition);
                break;
            }
        }
    }

    private void MoveByDirection(SceneConnectorInteraction.Direction direction)
    {
        if (direction.Equals(SceneConnectorInteraction.Direction.None)) return;
        Vector2 targetPosition = Manager.Instance.gameManager.player.transform.position;

        switch (direction)
        {
            case SceneConnectorInteraction.Direction.Up:
                targetPosition += Manager.Instance.gameManager.player.entityCollider.bounds.size.y * Vector2.up; break;
            case SceneConnectorInteraction.Direction.Down:
                targetPosition += Manager.Instance.gameManager.player.entityCollider.bounds.size.y * Vector2.down; break;
            case SceneConnectorInteraction.Direction.Left:
                targetPosition += Manager.Instance.gameManager.player.entityCollider.bounds.size.x * Vector2.left; break;
            case SceneConnectorInteraction.Direction.Right:
                targetPosition += Manager.Instance.gameManager.player.entityCollider.bounds.size.x * Vector2.right; break;
            default: break;
        }

        Player.Instance.movement.SetPosition(targetPosition);
    }

    private void ChangePosition(Transform destinationTranfrom)
    {
        if (destinationTranfrom == null) return;
        Player.Instance.movement.SetPosition(destinationTranfrom.position);
    }

    private void OnActiveSceneChanged(Scene current, Scene next)
    {
        Debug.Log($"Active scene changed from \"{current.name}\" to \"{next.name}\"");
        LoadAndUnloadScenes();
        FindDoor(doorToSpawnAt);
        MoveByDirection(direction);
        ChangePosition(destinationTransform);
        
        if (current.name != "" && next.name != "MainMenu" && next.name != "OpeningCutscene")
        {
            Player.Instance.gameObject.SetActive(true);
            statusBar.SetActive(true);
            cinemachineVirtualCamera.Follow = Player.Instance.transform;
            cinemachineVirtualCamera.ForceCameraPosition(Player.Instance.transform.position + Vector3.up * 25.0f, Quaternion.identity);
            cinemachineVirtualCamera.GetComponent<CinemachineConfiner>().m_BoundingShape2D = next.GetRootGameObjects().Where(gameObject => gameObject.name.Equals("Camera Boundary")).FirstOrDefault()?.GetComponent<PolygonCollider2D>();
            Manager.Instance.dataManager.SaveGame();
        }
        else
        {
            Player.Instance.gameObject.SetActive(false);
            statusBar.SetActive(false);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Loading Scene \"{scene.name}\" is done.");

        if (currentAsyncOperationDictionary.ContainsKey(scene.name))
        {
            currentAsyncOperationDictionary.Remove(scene.name);
        }

        if (!SceneManager.GetActiveScene().name.Equals(currentActiveScene.SceneName))
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentActiveScene.SceneName));
        }
    }

    // TODO: below part should be deleted later
    // saving will be supported on saving spots
    public void LoadData(GameData data)
    {

    }

    public void SaveData(GameData data)
    {
        if (!currentActiveScene.SceneName.Equals("MainMenu"))
        {
            data.currentScene = currentActiveScene.SceneName;
            // data system saves the game before loading the game
        }
    }
}