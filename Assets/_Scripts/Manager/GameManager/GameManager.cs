using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [field: SerializeField] public Player player { get; private set; }
    [SerializeField] private float cameraShakeForce = 20.0f;
    public bool isPaused { get; private set; }

    public event Action sceneClearedAction;
    public Trigger initialInvoke { get; private set; }

    private float originalTimeScale = 1.0f;
    private Coroutine pauseGameCoroutine;

    private void Awake()
    {
        initialInvoke = false;
    }

    private void Update()
    {
        Enemy[] currentSceneEnemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        bool enemyEliminated = true;


        foreach (Enemy enemy in currentSceneEnemies)
        {
            if (enemy.gameObject.scene.name.Equals(Manager.Instance.sceneTransitionManager.currentActiveScene.SceneName))
            {
                if (!enemy.isDead)
                {
                    enemyEliminated = false;
                    break;
                }
            }
        }

        if (enemyEliminated && currentSceneEnemies.Count() > 0 && initialInvoke.Value)
        {
            sceneClearedAction?.Invoke();
        }
    }

    public void PauseGame(float? duration = null)
    {
        if (Time.timeScale == 0.0f)
        { 
            Debug.Log("Already paused");
            return;
        }

        Debug.Log("Game Paused");
        originalTimeScale = Time.timeScale;
        isPaused = true;
        Time.timeScale = 0.0f;
    }

    public void ResumeGame()
    {
        Debug.Log("Game Resumed");
        isPaused = false;
        Time.timeScale = originalTimeScale;
    }

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public void PauseGame(float duration)
    {
        if (pauseGameCoroutine != null)
        {
            StopCoroutine(pauseGameCoroutine);
        }
        pauseGameCoroutine = StartCoroutine(PauseGameCoroutine(duration));
    }

    private IEnumerator PauseGameCoroutine(float duration)
    {
        PauseGame();

        yield return new WaitForSecondsRealtime(duration);
        
        ResumeGame();
    }

    public void CameraShake(CinemachineImpulseSource impulseSource)
    {
        impulseSource.GenerateImpulseWithForce(cameraShakeForce);
    }
}
