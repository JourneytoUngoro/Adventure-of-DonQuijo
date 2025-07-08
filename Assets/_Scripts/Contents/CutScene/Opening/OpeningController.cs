using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningController : MonoBehaviour
{
    public GameObject[] cutObjs;

    private ICutScene[] cutscenes;
    private int currentIndex;


    private void Awake()
    {
        cutscenes = new ICutScene[cutObjs.Length];
        for (int i = 0; i < cutObjs.Length; i++)
        {
            cutscenes[i] = cutObjs[i].GetComponent<ICutScene>();
            
            // initialize and inactive cutscene objects
            cutObjs[i].SetActive(false);
        }

        for (int i = 0; i < cutscenes.Length - 1; i++)
        {
            cutscenes[i].onFinish += (() =>
            {
                currentIndex++;
                PlayNextCut(currentIndex);
            });
        }

        currentIndex = 0;
    }

    public void PlayNextCut(int index)
    {
        cutObjs[index].SetActive(true);
        cutscenes[index].InitializeScene();
        cutscenes[index].PlayScene();
    }

    public void StartOpeningCutScene()
    {
        PlayNextCut(0);
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
}
