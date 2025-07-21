using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentController : MonoBehaviour
{
    public Toggle tutorialSkipToggle; // default : false
    public Toggle autoSaveToggle; // default : false



    public const string TUTORIAL_SKIP = "tutorialSkip";
    public const string AUTO_SAVE = "autoSave";

    private void Start()
    {
        InitializeToggles();
    }

    private void InitializeToggles()
    {
        // default value is false
        tutorialSkipToggle.isOn = PlayerPrefs.GetInt(TUTORIAL_SKIP, 0) == 0 ? false : true;
        autoSaveToggle.isOn = PlayerPrefs.GetInt(AUTO_SAVE, 0) == 0 ? false : true;
    }

    private void AddListenerToUI()
    {
        tutorialSkipToggle.onValueChanged.AddListener(isOn => SetTutorialSkipEnabled(isOn));
        autoSaveToggle.onValueChanged.AddListener(isOn => SetAutoSaveEnabled(isOn));
    }


    //===================================================== TODO ===========================================================================
    private void SetTutorialSkipEnabled(bool isOn)
    {
        // 얘는 굳이 할 필요 있나? LoadGameUI에서 해당 키 참조해서 PlayerPrefs로 , bool 확인 후 ?? 튜토리얼 끄든가 하면 될듯 
    }

    private void SetAutoSaveEnabled(bool isOn)
    {
        // TODO : DataManager의 자동저장 역할 로직 구현해야 하는데 이따 하기
        // 
    }
    //=========================================================================================================================================





}
