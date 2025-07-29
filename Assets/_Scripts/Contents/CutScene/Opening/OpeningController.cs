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
        Debug.Log("오프닝컷신 끝!");
        persistentObject.SetActive(true);
        Manager.Instance.sceneTransitionManager.SceneTransition(new SceneField("Stage1"), true, false);
        Manager.Instance.soundManager.PlayBGM("battleBGM");
    }

}
