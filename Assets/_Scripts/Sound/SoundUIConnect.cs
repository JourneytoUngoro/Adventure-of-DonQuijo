using UnityEngine;
using UnityEngine.UI;

public class SoundUIConnect : MonoBehaviour
{
    public SoundCategory category;
    public Slider volumSlider;

    private void Start()
    {
        float current = PlayerPrefs.GetFloat(category + "Volum", 1f);
        volumSlider.value = current;
        volumSlider.onValueChanged.AddListener(SetVolume);
    }

    private void SetVolume(float volume)
    {
        SoundManager.Instance.SetVolume(category, volume);
    }


}
