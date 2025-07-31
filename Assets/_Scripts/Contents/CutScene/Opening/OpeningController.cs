using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningController : MonoBehaviour
{
    public GameObject[] cutObjs;

    private ICutScene[] cutscenes;
    private int cutscenesCount;
    private int currentIndex;

    private GameObject persistentObject;

    private void Awake()
    {
        cutscenes = new ICutScene[cutObjs.Length];
        cutscenesCount = cutObjs.Length;

        for (int i = 0; i < cutscenesCount; i++)
        {
            cutscenes[i] = cutObjs[i].GetComponent<ICutScene>();
            
            // initialize and inactive cutscene objects
            cutObjs[i].SetActive(false);
        }

        for (int i = 0; i < cutscenesCount - 1; i++)
        {
            cutscenes[i].onFinish += (() =>
            {
                currentIndex++;
                PlayNextCut(currentIndex);
            });
        }

        // The last scene of Opening
        cutscenes[cutscenesCount - 1].onFinish += OnFinishOpening;

        currentIndex = 0;
    }

    private void Start()
    {
        persistentObject = GameObject.Find("Persistant GameObject");
        persistentObject.SetActive(false);

        StartOpeningCutScene();
    }

    private void OnDestroy()
    {
        // Disabled objects during the cutscene should be re-enabled after the cutscene ends.
        if (persistentObject != null && !persistentObject.activeSelf) persistentObject?.SetActive(true);
    }

    public void StartOpeningCutScene()
    {
        PlayNextCut(0);
    }

    public void PlayNextCut(int index)
    {
        cutObjs[index].SetActive(true);
        cutscenes[index].InitializeScene();
        cutscenes[index].PlayScene();
    }

    public void InitializeAllScene()
    {
        for (int i = 0; i < cutObjs.Length; i++)
        {
            cutscenes[i].InitializeScene();
            cutObjs[i].SetActive(false);
        }
        currentIndex = 0;
    }

    private void OnFinishOpening()
    {
        persistentObject.SetActive(true);
        Manager.Instance.sceneTransitionManager.SceneTransition(new SceneField("Stage1"), true, false);
        Manager.Instance.soundManager.PlayBGMTransition("battleBGM", 0.3f, 0.03f, 0.5f);
    }

}
