using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionController : MonoBehaviour
{
    public TMP_Dropdown resolutionDrop;

    private Resolution[] resolutions;
    

    private void Awake()
    {
        // get all resolutions of player
        resolutions = Screen.resolutions;

        InitResolutionDrop();
        AddListenerToDrop();
    }

    private void InitResolutionDrop()
    {
        // clear
        resolutionDrop.ClearOptions();

        HashSet<string> options = new HashSet<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; ++i)
        {
            string optioin = $"{resolutions[i].width} X {resolutions[i].height}";
            options.Add(optioin);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDrop.AddOptions(new List<string>(options));
        resolutionDrop.value = currentResolutionIndex;
        resolutionDrop.RefreshShownValue();
    }

    private void AddListenerToDrop()
    {
        resolutionDrop.onValueChanged.AddListener(value => SetCurrentResolution(value));
    }

    public void SetCurrentResolution(int chosenIndex)
    {
        Resolution chosenResolution = resolutions[chosenIndex];
        Screen.SetResolution(chosenResolution.width, chosenResolution.height, Screen.fullScreen);

        Debug.Log($"Resoultion changed -> {chosenResolution}");
    }

}
