using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentController : MonoBehaviour
{
    public Toggle tutorialSkipToggle; // default : false
    public Toggle autoSaveToggle;    // default : true
    public Toggle cutsceneSkip;       // default : false

    public static string TUTORIAL_SKIP = "tutorialSkip";
    public static string AUTO_SAVE = "autoSave";
    public static string CUTSCENE_SKIP = "cutsceneSkip";

    private void Start()
    {
        InitializeToggles();

        AddListenerToUI();
    }

    private void InitializeToggles()
    {
        // default value is false
        tutorialSkipToggle.isOn = PlayerPrefs.GetInt(TUTORIAL_SKIP, 0) == 0 ? false : true;
        autoSaveToggle.isOn = PlayerPrefs.GetInt(AUTO_SAVE, 1) == 0 ? false : true;
        cutsceneSkip.isOn = PlayerPrefs.GetInt(CUTSCENE_SKIP, 0) == 0 ? false : true;
    }

    private void AddListenerToUI()
    {
        tutorialSkipToggle.onValueChanged.AddListener(isOn => SetTutorialSkipEnabled(isOn));
        autoSaveToggle.onValueChanged.AddListener(isOn => SetAutoSaveEnabled(isOn));
        cutsceneSkip.onValueChanged.AddListener(isOn => SetCutsceneSkipEnabled(isOn));
    }

    private void SetTutorialSkipEnabled(bool isOn)
    {
        int isOnValue = isOn ? 1 : 0;
        PlayerPrefs.SetInt(TUTORIAL_SKIP, isOnValue);
    }

    private void SetAutoSaveEnabled(bool isOn)
    {
        int isOnValue = isOn ? 1 : 0;
        PlayerPrefs.SetInt(AUTO_SAVE, isOnValue);

        // isOn means Enable
        Manager.Instance.dataManager.SetAutoSaveDisable(!isOn);

        Debug.Log($"toggle {isOn}, set value {isOnValue}");

    }

    private void SetCutsceneSkipEnabled(bool isOn)
    {
        int isOnValue = isOn ? 1 : 0;
        PlayerPrefs.SetInt(CUTSCENE_SKIP, isOnValue);
    }
}
