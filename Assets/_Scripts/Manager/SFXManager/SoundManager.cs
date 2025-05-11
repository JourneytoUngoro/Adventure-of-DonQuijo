using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundCategory
{
    Master, 
    BGM,
    SFX,
    Dialogue,
    UI,
}

public class SoundManager : MonoBehaviour
{

    // TODO : Magnager에 통합
    public static SoundManager Instance { get; private set; }

    [TabGroup("Tab/Audio Mixer")][SerializeField] private AudioMixer audioMixer;
    [TabGroup("Tab/Audio Mixer")][SerializeField] private AudioMixerGroup bgmGroup;
    [TabGroup("Tab/Audio Mixer")][SerializeField] private AudioMixerGroup sfxGroup;
    [TabGroup("Tab/Audio Mixer")][SerializeField] private AudioMixerGroup uiGroup;

    [TabGroup("Tab/BGM")][SerializeField] private AudioSource bgmSource;

    [Title("Sound Player Pool")][SerializeField] private GameObject soundPlayerPrefab;
    [SerializeField] private int poolSize = 30;
    private Queue<SoundPlayer> pool = new Queue<SoundPlayer>();

    private const string MASTER_VOL_KEY = "MasterVolume";
    private const string BGM_VOL_KEY = "BGMVolume";
    private const string SFX_VOL_KEY = "SFXVolume";
    private const string UI_VOL_KEY = "UIVolume";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        InitPool();
        LoadVolume();
    }

    private void InitPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            SoundPlayer sp = Instantiate(soundPlayerPrefab, transform).GetComponent<SoundPlayer>();
            pool.Enqueue(sp);
        }
    }

    private SoundPlayer GetSoundPlayer()
    {
        return pool.Count > 0 ?
            pool.Dequeue() :
            Instantiate(soundPlayerPrefab, transform).GetComponent<SoundPlayer>();
    }

    private void ReturnSoundPlayer(SoundPlayer sp)
    {
        pool.Enqueue(sp);
    }

    private void Play(AudioClip clip, AudioMixerGroup group, float volume)
    {
        if (clip == null)
        {
            Debug.LogWarning("clip is null!");
            return;
        }

        SoundPlayer sp = GetSoundPlayer();
        sp.Play(clip, group, volume, ReturnSoundPlayer);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f) => Play(clip, sfxGroup, volume);

    public void PlayUI(AudioClip clip, float volume = 1f) => Play(clip, uiGroup, volume);

    public void PlayBGM(AudioClip clip)
    {
        if ( bgmSource.isPlaying) 
        {
            bgmSource.Stop();
        }

        bgmSource.clip = clip;
        bgmSource.outputAudioMixerGroup = bgmGroup;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void SetVolume(SoundCategory category, float inputVolume)
    {
        float volume = Mathf.Clamp01(inputVolume);
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;

        string exposedName = category switch
        {
            SoundCategory.Master => "MasterVolume",
            SoundCategory.BGM => "BGMVolume",
            SoundCategory.SFX => "SFXVolume",
            SoundCategory.UI => "UIVolume",
            _ => "MasterVolume"
        };

        audioMixer.SetFloat(exposedName, dB);
        PlayerPrefs.SetFloat(exposedName, volume);
    }

    public void LoadVolume()
    {
        SetVolume(SoundCategory.Master, PlayerPrefs.GetFloat(MASTER_VOL_KEY, 1f));
        SetVolume(SoundCategory.BGM, PlayerPrefs.GetFloat(BGM_VOL_KEY, 1f));
        SetVolume(SoundCategory.SFX, PlayerPrefs.GetFloat(SFX_VOL_KEY, 1f));
        SetVolume(SoundCategory.UI, PlayerPrefs.GetFloat(UI_VOL_KEY, 1f));
    }

}
