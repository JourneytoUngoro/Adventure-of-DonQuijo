using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestVendingMachine : MonoBehaviour
{
    public static bool VENDINGMACHINE_SCENE;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"테스트 !!!!!!!!!!!!!!!!!!! 매니저 !! 지금 로드된 씬 : {scene.name}");
    }
}
