using Ink.Parsed;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoad : MonoBehaviour
{
    public SceneField excludedScene;
    public List<GameObject> childs;

    private static DontDestroyOnLoad instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
       if (scene.name.Equals(excludedScene.SceneName))
        {
            foreach (GameObject child in childs)
            {
                child.gameObject.SetActive(false);
            }
        }
       else
        {
            foreach (GameObject child in childs)
            {
                child.gameObject.SetActive(true);
            }
        }

    }



}
