using UnityEngine;
using UnityEngine.UI;

public class SoundUIConnect : MonoBehaviour
{
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Slider uiSlider;

    public Toggle masterToggle;
    public Toggle bgmToggle;
    public Toggle sfxToggle;
    public Toggle uiToggle;

    private const float minValue = 0.0001f;
    private float epsilon = 0.001f;

    private void Start()
    {
        AddListnersToUI();
        InitializeUIValue();
    }

    private void InitializeUIValue()
    {
        masterSlider.value = Manager.Instance.soundManager.GetVolumeValue(SoundCategory.Master);
        bgmSlider.value = Manager.Instance.soundManager.GetVolumeValue(SoundCategory.BGM);
        sfxSlider.value = Manager.Instance.soundManager.GetVolumeValue(SoundCategory.SFX);
        uiSlider.value = Manager.Instance.soundManager.GetVolumeValue(SoundCategory.UI);

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
    
    // slider
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
    }

    // toggle 
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
        // todo : Should add UI state change logic
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
        // todo : Should add UI state change logic
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
        // todo : Should add UI state change logic
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
        // todo : Should add UI state change logic
    }



}
