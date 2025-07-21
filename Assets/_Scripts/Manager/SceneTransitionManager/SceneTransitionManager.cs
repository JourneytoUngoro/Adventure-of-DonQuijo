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
    private IEnumerator FadeOut(bool useLoadingBar, string loadingScene)
    {
        Manager.Instance.gameManager.PauseGame();

        if (Manager.Instance.player?.gameObject.activeSelf == true)
        {
            Manager.Instance.inputHandler.playerInput.currentActionMap.Disable();
        }

        ImageUI fadeInOutImage = Manager.Instance.uiManager.GetUI<ImageUI>(UIType.FadeImage);

        if (useLoadingBar)
        {
            fadeInOutImage.ShowUI(() => LoadingBar(loadingScene));
        }
        else
        {
            fadeInOutImage.ShowUI(null);
        }

        yield return new WaitForSecondsRealtime(minDuration);
        yield return new WaitUntil(() => SceneManager.GetSceneByName(loadingScene).isLoaded);

        fadeInOutImage.HideUI(() => Manager.Instance.gameManager.ResumeGame());
    }

    public async void LoadingBar(string loadingScene)
    {
        AsyncOperation asyncOperation;

        loadingBar.gameObject.SetActive(true);
        loadingBarText.text = "Game Tips | " + loadingText.GetRandom();

        if (!currentAsyncOperationDictionary.TryGetValue(loadingScene, out asyncOperation))
        {
            Debug.Log($"Can't find async operation of {loadingScene}. Inactivate loading bar.");
            currentProgress = 1.0f;
        }
        else
        {
            currentProgress = 0.0f;

            do
            {
                currentProgress = asyncOperation.progress;
                await Task.Delay(100);
            } while (asyncOperation.progress < 1.0f);
        }
    }

    public async void SceneTransition(SceneField targetScene, DoorTriggerInteraction.DoorToSpawnAt doorToSpawnAt, bool useFadeInOut = false, bool useLoadingBar = false)
    {
        this.doorToSpawnAt = doorToSpawnAt;
        this.direction = SceneConnectorInteraction.Direction.None;
        this.destinationTransform = null;

        if (useFadeInOut)
        {
            FadeOut(useLoadingBar, targetScene.SceneName);
        }

        // if scene is not in SceneManager, it means that the scene is neither loaded nor currently loading
        if (!IsLoadingScene(targetScene.SceneName))
        {
            Debug.Log("Scene was not loaded. Start Loading the target scene: " + targetScene.SceneName);
            currentAsyncOperationDictionary.Add(targetScene.SceneName, SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive));
        }

        if (!SceneManager.GetSceneByName(targetScene).isLoaded)
        {
            Debug.Log($"Target scene \"{targetScene.SceneName}\" is not done loading.");
            Manager.Instance.gameManager.PauseGame();
            while (!currentAsyncOperationDictionary[targetScene.SceneName].isDone)
            {
                await Task.Delay(100);
            }
            Manager.Instance.gameManager.ResumeGame();
            Debug.Log($"Target scene \"{targetScene.SceneName}\" is done loading. Set as active scene.");
            currentActiveScene = targetScene;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetScene.SceneName));
        }
        else
        {
            currentActiveScene = targetScene;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetScene.SceneName));
        }

        this.useLoadingBar = useLoadingBar;
        if (useLoadingBar)
        {
            LoadingBar(targetScene.SceneName);
        }

        currentActiveScene = targetScene;
    }

    public async void SceneTransition(SceneField targetScene, SceneConnectorInteraction.Direction direction, bool useFadeInOut = false, bool useLoadingBar = false)
    {
        this.doorToSpawnAt = DoorTriggerInteraction.DoorToSpawnAt.None;
        this.direction = direction;
        this.destinationTransform = null;

        if (useFadeInOut)
        {
            FadeOut(useLoadingBar, targetScene.SceneName);
        }

        // if scene is not in SceneManager, it means that the scene is neither loaded nor currently loading
        if (!IsLoadingScene(targetScene.SceneName))
        {
            Debug.Log("Scene was not loaded. Start Loading the target scene: " + targetScene.SceneName);
            currentAsyncOperationDictionary.Add(targetScene.SceneName, SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive));
        }

        if (!SceneManager.GetSceneByName(targetScene).isLoaded)
        {
            Debug.Log($"Target scene \"{targetScene.SceneName}\" is not done loading.");
            Manager.Instance.gameManager.PauseGame();
            while (!currentAsyncOperationDictionary[targetScene.SceneName].isDone)
            {
                await Task.Delay(100);
            }

            if (!useFadeInOut)
            {
                Manager.Instance.gameManager.ResumeGame();
            }
            Debug.Log($"Target scene \"{targetScene.SceneName}\" is done loading. Set as active scene.");
            currentActiveScene = targetScene;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetScene.SceneName));
        }
        else
        {
            currentActiveScene = targetScene;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetScene.SceneName));
        }
    }

    /*public async void SceneTransition(SceneField targetScene, Transform destinationTransform, bool useFadeInOut = false, bool useLoadingBar = false)
    {
        this.doorToSpawnAt = DoorTriggerInteraction.DoorToSpawnAt.None;
        this.direction = SceneConnectorInteraction.Direction.None;
        this.destinationTransform = destinationTransform;

        // if scene is not in SceneManager, it means that the scene is neither loaded nor currently loading
        if (!IsLoadingScene(targetScene.SceneName))
        {
            Debug.Log("Scene was not loaded. Start Loading the target scene: " + targetScene.SceneName);
            currentAsyncOperationDictionary.Add(targetScene.SceneName, SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive));
        }

        if (useFadeInOut)
        {
            FadeOut(useLoadingBar, targetScene.SceneName);
        }

        if (!SceneManager.GetSceneByName(targetScene).isLoaded)
        {
            Debug.Log($"Target scene \"{targetScene.SceneName}\" is not done loading.");
            Manager.Instance.gameManager.PauseGame();
            while (!currentAsyncOperationDictionary[targetScene.SceneName].isDone)
            {
                await Task.Delay(100);
            }
            Manager.Instance.gameManager.ResumeGame();
            Debug.Log($"Target scene \"{targetScene.SceneName}\" is done loading. Set as active scene.");
            currentActiveScene = targetScene;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetScene.SceneName));
        }
        else
        {
            currentActiveScene = targetScene;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetScene.SceneName));
        }
    }*/

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

        /*if (useFadeInOut)
        {
            FadeOut(useLoadingBar, targetScene.SceneName);
        }

        if (!IsLoadingScene(targetScene.SceneName))
        {
            Debug.Log("Scene was not loaded. Start Loading the target scene: " + targetScene.SceneName);
            currentAsyncOperationDictionary.Add(targetScene.SceneName, SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive));
        }

        if (!SceneManager.GetSceneByName(targetScene).isLoaded)
        {
            Debug.Log($"Target scene \"{targetScene.SceneName}\" is not done loading.");
            Manager.Instance.gameManager.PauseGame();
            while (!currentAsyncOperationDictionary[targetScene.SceneName].isDone)
            {
                await Task.Delay(100);
            }
            Manager.Instance.gameManager.ResumeGame();
            Debug.Log($"Target scene \"{targetScene.SceneName}\" is done loading. Set as active scene.");
            currentActiveScene = targetScene;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetScene.SceneName));
        }
        else
        {
            currentActiveScene = targetScene;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetScene.SceneName));
        }*/
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
        if (!IsLoadingScene(targetScene))
        {
            Debug.Log("Scene was not loaded. Start Loading the target scene: " + targetScene);
            currentAsyncOperationDictionary.Add(targetScene, SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive));
        }

        if (useFadeInOut)
        {
            if (fadeInOutCoroutine != null)
            {
                StopCoroutine(fadeInOutCoroutine);
            }
            yield return StartCoroutine(FadeOut(useLoadingBar, targetScene));
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

    public async void SceneTransition(string targetScene, bool useFadeInOut = false, bool useLoadingBar = false)
    {
        this.doorToSpawnAt = DoorTriggerInteraction.DoorToSpawnAt.None;
        this.direction = SceneConnectorInteraction.Direction.None;
        this.destinationTransform = null;

        // if scene is not in SceneManager, it means that the scene is neither loaded nor currently loading
        if (!IsLoadingScene(targetScene))
        {
            Debug.Log("Scene was not loaded. Start Loading the target scene: " + targetScene);
            currentAsyncOperationDictionary.Add(targetScene, SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive));
        }

        if (useFadeInOut)
        {
            StartCoroutine(FadeOut(useLoadingBar, targetScene));
        }

        if (!SceneManager.GetSceneByName(targetScene).isLoaded)
        {
            Debug.Log($"Target scene \"{targetScene}\" is not done loading.");
            Manager.Instance.gameManager.PauseGame();
            while (!currentAsyncOperationDictionary[targetScene].isDone)
            {
                await Task.Delay(100);
            }
            Manager.Instance.gameManager.ResumeGame();
            Debug.Log($"Target scene \"{targetScene}\" is done loading. Set as active scene.");
            currentActiveScene = new SceneField(targetScene);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetScene));
        }
        else
        {
            currentActiveScene = new SceneField(targetScene);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetScene));
        }
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
                Manager.Instance.player.movement.SetPosition(groundPosition);
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

        Manager.Instance.player.movement.SetPosition(targetPosition);
    }

    private void ChangePosition(Transform destinationTranfrom)
    {
        if (destinationTranfrom == null) return;
        Manager.Instance.player.movement.SetPosition(destinationTranfrom.position);
    }

    private async void OnActiveSceneChanged(Scene current, Scene next)
    {
        while (isFadingOut)
        {
            await Task.Delay(100);
        }

        /*while (Manager.Instance.gameManager.player == null)
        {
            await Task.Delay(10);
        }*/

        Debug.Log($"Active scene changed from \"{current.name}\" to \"{next.name}\"");
        LoadAndUnloadScenes();
        FindDoor(doorToSpawnAt);
        MoveByDirection(direction);
        ChangePosition(destinationTransform);
        
        if (Manager.Instance.player != null)
        {
            cinemachineVirtualCamera.Follow = Manager.Instance.player.transform;
            cinemachineVirtualCamera.ForceCameraPosition(Manager.Instance.player.transform.position + Vector3.up * 25.0f, Quaternion.identity);
            cinemachineVirtualCamera.GetComponent<CinemachineConfiner>().m_BoundingShape2D = next.GetRootGameObjects().Where(gameObject => gameObject.name.Equals("Camera Boundary")).FirstOrDefault()?.GetComponent<PolygonCollider2D>();
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