using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance { get; private set; }
    public PlayerInputHandler inputHandler { get; private set; }
    public GameManager gameManager { get; private set; }
    public ObjectPoolingManager objectPoolingManager { get; private set; }

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        #endregion

        inputHandler = GetComponentInChildren<PlayerInputHandler>();
        gameManager = GetComponentInChildren<GameManager>();
        objectPoolingManager = GetComponentInChildren<ObjectPoolingManager>();
    }
}
