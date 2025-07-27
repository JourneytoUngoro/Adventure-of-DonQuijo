using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverlayUI : MonoBehaviour
{
    public static OverlayUI Instance { get; private set; }

    private void Awake()
    {
        #region singleton
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(Instance);
        #endregion


    }
}
