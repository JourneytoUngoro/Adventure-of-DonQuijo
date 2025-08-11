using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractBaseController : MonoBehaviour
{
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
        Player.Instance.interactableGameObjects.RemoveAll(interactable => interactable.interactionType == InteractBase.InteractionType.Object);
        Debug.Log($"InteractBase : 지운 후 {Player.Instance.interactableGameObjects.Count} 개");
    }
}
