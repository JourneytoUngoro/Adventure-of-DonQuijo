using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [field: SerializeField] public Player player { get; private set; }
    [SerializeField] public float cameraShakeForce = 20.0f;
    public bool isPaused { get; private set; }
    private float originalTimeScale = 1.0f;
    private Coroutine pauseGameCoroutine;

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
