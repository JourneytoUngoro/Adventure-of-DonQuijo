using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundUIConnect : MonoBehaviour
{
    #region Sound Variables
    [TabGroup("Slider"), LabelText("Master Volume")]
    public Slider masterSlider;
    [TabGroup("Slider"), LabelText("BGM Volume")]
    public Slider bgmSlider;
    [TabGroup("Slider"), LabelText("SFX Volume")]
    public Slider sfxSlider;
    [TabGroup("Slider"), LabelText("UI Volume")]
    public Slider uiSlider;

    [TabGroup("Toggle"), LabelText("Master Mute")]
    public Toggle masterToggle;
    [TabGroup("Toggle"), LabelText("BGM Mute")]
    public Toggle bgmToggle;
    [TabGroup("Toggle"), LabelText("SFX Mute")]
    public Toggle sfxToggle;
    [TabGroup("Toggle"), LabelText("UI Mute")]
    public Toggle uiToggle;

    [TabGroup("TMP"), LabelText("Master Volume Text")]
    public TextMeshProUGUI masterVolumeTMP;
    [TabGroup("TMP"), LabelText("BGM Volume Text")]
    public TextMeshProUGUI bgmVolumeTMP;
    [TabGroup("TMP"), LabelText("SFX Volume Text")]
    public TextMeshProUGUI sfxVolumeTMP;
    [TabGroup("TMP"), LabelText("UI Volume Text")]
    public TextMeshProUGUI uiVolumeTMP;
    #endregion

    private const float minValue = 0.0001f;
    private float epsilon = 0.001f;

    private void Start()
    {
        AddListnersToUI();
        InitializeUIValue();
    }

    private void InitializeUIValue()
    {
        // set slider value by current volume
        masterSlider.value = Manager.Instance.soundManager.GetVolumeValue(SoundCategory.Master);
        bgmSlider.value = Manager.Instance.soundManager.GetVolumeValue(SoundCategory.BGM);
        sfxSlider.value = Manager.Instance.soundManager.GetVolumeValue(SoundCategory.SFX);
        uiSlider.value = Manager.Instance.soundManager.GetVolumeValue(SoundCategory.UI);

        // set volume text by current volume
        masterVolumeTMP.text = Mathf.Round(Manager.Instance.soundManager.GetVolumeValue(SoundCategory.Master) * 100f).ToString();
        bgmVolumeTMP.text = Mathf.Round(Manager.Instance.soundManager.GetVolumeValue(SoundCategory.BGM) * 100f).ToString();
        sfxVolumeTMP.text = Mathf.Round(Manager.Instance.soundManager.GetVolumeValue(SoundCategory.SFX) * 100f).ToString();
        uiVolumeTMP.text = Mathf.Round(Manager.Instance.soundManager.GetVolumeValue(SoundCategory.UI) * 100f).ToString();

        // set toggle by slider value
        masterToggle.isOn = masterSlider.value != minValue;
        bgmToggle.isOn = bgmSlider.value != minValue;
        sfxToggle.isOn = sfxSlider.value != minValue;
        uiToggle.isOn = uiSlider.value != minValue;
    }

    private void AddListnersToUI()
    {
        masterSlider.onValueChanged.AddListener(value => SetMasterVolume(value));
        bgmSlider.onValueChanged.AddListener(value => SetBGMVolume(value));
        sfxSlider.onValueChanged.AddListener(value => SetSFXVolume(value));
        uiSlider.onValueChanged.AddListener(value => SetUIVolume(value));

        masterToggle.onValueChanged.AddListener(value => SetMasteOn(value));
        bgmToggle.onValueChanged.AddListener(value => SetBGMOn(value));
        sfxToggle.onValueChanged.AddListener(value => SetSFXOn(value));
        uiToggle.onValueChanged.AddListener(value => SetUIOn(value));
    }

    // slider : set volume
    private void SetMasterVolume(float volume)
    {
        if (volume < minValue + epsilon)
        {
            masterToggle.isOn = false;
        }
        else
        {
            if (!masterToggle.isOn) masterToggle.isOn = true;
            Manager.Instance.soundManager.SetVolume(SoundCategory.Master, volume);
        }

        masterVolumeTMP.text = Mathf.Round(Manager.Instance.soundManager.GetVolumeValue(SoundCategory.Master) * 100f).ToString();
    }

    private void SetBGMVolume(float volume)
    {
        if (volume < minValue + epsilon)
        {
            bgmToggle.isOn = false;
        }
        else
        {
            if (!bgmToggle.isOn) bgmToggle.isOn = true;
            Manager.Instance.soundManager.SetVolume(SoundCategory.BGM, volume);
        }

        bgmVolumeTMP.text = Mathf.Round(Manager.Instance.soundManager.GetVolumeValue(SoundCategory.BGM) * 100f).ToString();
    }

    private void SetSFXVolume(float volume)
    {
        if (volume < minValue + epsilon)
        {
            sfxToggle.isOn = false;
        }
        else
        {
            if (!sfxToggle.isOn) sfxToggle.isOn = true;
            Manager.Instance.soundManager.SetVolume(SoundCategory.SFX, volume);
        }

        sfxVolumeTMP.text = Mathf.Round(Manager.Instance.soundManager.GetVolumeValue(SoundCategory.SFX) * 100f).ToString();
    }

    private void SetUIVolume(float volume)
    {
        if (volume < minValue + epsilon)
        {
            uiToggle.isOn = false;
        }
        else
        {
            if (!uiToggle.isOn) uiToggle.isOn = true;
            Manager.Instance.soundManager.SetVolume(SoundCategory.UI, volume);
        }

        uiVolumeTMP.text = Mathf.Round(Manager.Instance.soundManager.GetVolumeValue(SoundCategory.UI) * 100f).ToString();
    }

    // toggle : mute sounds
    // TODO : toggle UI needs needs to add UI logic
    private void SetMasteOn(bool isOn)
    {
        float newVolume;
        if (isOn)
        {
            newVolume = masterSlider.value;
        }
        else
        {
            newVolume = minValue;
        }

        Manager.Instance.soundManager.SetVolume(SoundCategory.Master, newVolume);
    }

    private void SetBGMOn(bool isOn)
    {
        float newVolume;
        if (isOn)
        {
            newVolume = bgmSlider.value;
        }
        else
        {
            newVolume = minValue;
        }

        Manager.Instance.soundManager.SetVolume(SoundCategory.BGM, newVolume);
    }

    private void SetSFXOn(bool isOn)
    {
        float newVolume;
        if (isOn)
        {
            newVolume = sfxSlider.value;
        }
        else
        {
            newVolume = minValue;
        }

        Manager.Instance.soundManager.SetVolume(SoundCategory.SFX, newVolume);
    }

    private void SetUIOn(bool isOn)
    {
        float newVolume;
        if (isOn)
        {
            newVolume = uiSlider.value;
        }
        else
        {
            newVolume = minValue;
        }

        Manager.Instance.soundManager.SetVolume(SoundCategory.UI, newVolume);
    }
} 