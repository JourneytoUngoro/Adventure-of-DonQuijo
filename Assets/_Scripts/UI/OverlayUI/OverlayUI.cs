using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayUI : MonoBehaviour
{
    public static OverlayUI Instance {  get; private set; }

    private void Awake()
    {
        #region singleton
        if (Instance != null)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(Instance);
        #endregion


    }
}
