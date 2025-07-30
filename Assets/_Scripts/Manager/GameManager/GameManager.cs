using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [field: SerializeField] public Player player { get; private set; }
    public bool isPaused { get; private set; }
    private float originalTimeScale = 1.0f;

    public void PauseGame()
    {
        if (Time.timeScale == 0.0f) { Debug.Log("Already paused"); return; }

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

    // ==================== TODO : TEST CODE 삭제해 ====================
    public bool PRINT = false;
    private void Update()
    {
        if (PRINT)
            Debug.Log($"Time.timeScale : {Time.timeScale}, OriginalTimeScale : {originalTimeScale}, isPaused : {isPaused}");
    }
}
