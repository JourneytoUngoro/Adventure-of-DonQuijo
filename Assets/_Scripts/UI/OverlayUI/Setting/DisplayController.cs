using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayController : MonoBehaviour
{
    #region Display Variables
    [TabGroup("Resolution"), LabelText("Resolution Dropdown")]
    public TMP_Dropdown resolutionDrop;
    private Resolution[] resolutions;
    private Resolution currentResolution;

    [TabGroup("Screen Mode"), LabelText("Fullscreen Toggle")]
    public Toggle fullScreenToggle;
    [TabGroup("Screen Mode"), LabelText("Windowed Toggle")]
    public Toggle windowScreenToggle;

    [TabGroup("Brightness"), LabelText("Brightness Overlay Image")]
    public Image brightControlImage;
    [TabGroup("Brightness"), LabelText("Brightness Slider")]
    public Slider brightnessSlider;
    [TabGroup("Brightness"), LabelText("Brightness Text")]
    public TextMeshProUGUI currentBrightTMP;
    private float brightAlpha;
    private float brightMin, brightMax;

    [TabGroup("Frame")]
    public TMP_Dropdown frameDrop;
    private readonly int[] frames = { 30, 60, 144, -1 };
    private int currentFps;
    #endregion

    private const float cMaxAlpha = 255f;

    private static bool isInitialized = false;

    private void Awake()
    {
        if (isInitialized) return;
        fullScreenToggle.onValueChanged.RemoveAllListeners();

        Initialize();

        // get all resolutions of player
        resolutions = Screen.resolutions;

        InitResolutionDrop();
        InitFrameDrop();
        AddListenerToUI();

        isInitialized = true;
    }

    private void Initialize()
    {
        // resolution
        currentResolution = Screen.currentResolution;
        
        // screen mode
        if (Screen.fullScreen)
        {
            fullScreenToggle.isOn = true;
        }
        else
        {
            windowScreenToggle.isOn = true;
        }

        // brightness
        currentBrightTMP.text = "100";
        brightnessSlider.value = brightnessSlider.maxValue;
        brightMin = brightnessSlider.minValue;
        brightMax = brightnessSlider.maxValue;

        // frame
        QualitySettings.vSyncCount = 0; // vSyncCount must be disabled 'Application.targetFrameRate' to work properly
        currentFps = (int) (1f / Time.deltaTime);
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

    private void InitFrameDrop()
    {
        frameDrop.ClearOptions() ;

        HashSet<string> options = new HashSet<string>();

        int currentFrameIndex = -1;  

        for (int i = 0; i < frames.Length-1; ++ i)
        {
            string option = $"{frames[i]} fps";
            options.Add(option);

            if (frames[i] == currentFps)
            {
                currentFrameIndex = i;
            }
        }

        options.Add("프레임 제한 없음");

        frameDrop.AddOptions(new List<string>(options));
        if (currentFrameIndex == -1)
        {
            currentFrameIndex = 1;
            Application.targetFrameRate = frames[currentFrameIndex];
        }
        frameDrop.value = currentFrameIndex;
        frameDrop.RefreshShownValue();
    }

    private void AddListenerToUI()
    {
        resolutionDrop.onValueChanged.AddListener(value => SetCurrentResolution(value));

        fullScreenToggle.onValueChanged.AddListener(isOn => SetCurrentScreenMode(isOn));

        brightnessSlider.onValueChanged.AddListener(value => SetCurrentBrightness(value));

        frameDrop.onValueChanged.AddListener((value => SetCurrentFPS(value)));
    }

    public void SetCurrentResolution(int chosenIndex)
    {
        Resolution chosenResolution = resolutions[chosenIndex];
        Screen.SetResolution(chosenResolution.width, chosenResolution.height, Screen.fullScreen);

        Debug.Log($"Resoultion changed -> {chosenResolution}");
    }

    public void SetCurrentScreenMode(bool isOn)
    {
        if (isOn) // full screen mode
        {
            Screen.SetResolution(currentResolution.width, currentResolution.height, true);
        }
        else // window screen mode 
        {
            Screen.SetResolution(currentResolution.width, currentResolution.height, false);
        }

        Debug.Log($"current display info : {Screen.currentResolution} / {(Screen.fullScreen ? "full screen mode" : "window screen mode")} ");
    }

    public void SetCurrentBrightness(float brightness)
    {
        // Set the alpha of black Image on Canvas
        float defaultAlpha = cMaxAlpha - brightMax;
        float newAlpha = 1f - ((defaultAlpha + brightness) / cMaxAlpha);
        Color newColor = brightControlImage.color;
        newColor.a = newAlpha;
        brightControlImage.color = newColor;

        currentBrightTMP.text = ((int)brightness).ToString();
    }

    public void SetCurrentFPS(float fps)
    {
        int newFps = fps switch
        {
            0 => frames[0], // 30 fps
            1 => frames[1], // 60 fps
            2 => frames[2], // 144 fps
            3 => frames[3], // no limits
            _ => frames[1] // default : 60 fps
        };

        Application.targetFrameRate = newFps;
    }

}
