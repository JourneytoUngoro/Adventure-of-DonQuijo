using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [TabGroup("Tab", "Audio Mixer")][SerializeField] private AudioMixer audioMixer;
    [TabGroup("Tab", "Audio Mixer")][SerializeField] private AudioMixerGroup bgmGroup;
    [TabGroup("Tab", "Audio Mixer")][SerializeField] private AudioMixerGroup sfxGroup;
    [TabGroup("Tab", "Audio Mixer")][SerializeField] private AudioMixerGroup uiGroup;

    [TabGroup("Tab", "BGM")][SerializeField] private AudioSource bgmSource;

    [Title("Clip Database")] [SerializeField] private AudioClipDatabase clipDatabase;
    [ShowInInspector] private Dictionary<string, AudioClip> clipDictionary;

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

        InitSoundClips();
        InitPool();
        LoadVolume();
    }
    private void InitSoundClips()
    {
        clipDictionary = new Dictionary<string, AudioClip>();

        foreach (var entry in clipDatabase.soundClips)
        {
            if (!clipDictionary.ContainsKey(entry.key))
                clipDictionary.Add(entry.key, entry.clip);
            else
                Debug.LogWarning($"Duplicate sound key detected: {entry.key}");
        }
    }

    private void InitPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            SoundPlayer sp = Instantiate(soundPlayerPrefab, transform).GetComponent<SoundPlayer>();
            sp.gameObject.SetActive(false);
            pool.Enqueue(sp);
        }
    }


    private void Play(string key, AudioMixerGroup group, float volume)
    {
        AudioClip clip = GetClip(key);

        if (clip == null)
        {
            Debug.LogWarning("clip is null!");
            return;
        }

        SoundPlayer sp = GetSoundPlayer();
        sp.gameObject.SetActive(true);
        sp.Play(clip, group, volume, ReturnSoundPlayer);
    }

    // sfx player 
    private void Play(string key, AudioMixerGroup group, Transform spawnTransform, float pitchDeviation = 0.0f, float volume = 1.0f)
    {
        AudioClip clip = GetClip(key);

        if (clip == null)
        {
            Debug.LogWarning("clip is null!");
            return;
        }

        SoundPlayer sp = GetSoundPlayer();
        sp.gameObject.SetActive(true);
        sp.Play(clip, group, volume, ReturnSoundPlayer, spawnTransform, pitchDeviation);
    }

    public void PlaySoundFXClip(string key, Transform spawnTransform, float pitchDeviation = 0.0f, float volume = 1.0f)
    {
        float pitch = 1.0f + UtilityFunctions.RandomFloat(-pitchDeviation, pitchDeviation);

        Play(key, sfxGroup, spawnTransform, pitch, volume);
    }

    // TODO 함수 추가하기
/*    public void PlaySoundFXClip(IEnumerable<AudioClip> audioClips, Transform spawnTransform, float pitchDeviation = 0.0f, float volume = 1.0f)
    {
        if (audioClips == null)
        {
            Debug.LogWarning($"{audioClips} is null. Cannot play sound.");
            return;
        }

        foreach (AudioClip audioClip in audioClips)
        {
            if (audioClip == null)
            {
                Debug.LogWarning($"{audioClip.name} is null. Cannot play sound.");
                return;
            }
        }

        int index = UtilityFunctions.RandomInteger(audioClips.Count());

        AudioClip clip = audioClips.ElementAt(index);

       float pitch = 1.0f + UtilityFunctions.RandomFloat(-pitchDeviation, pitchDeviation);

    }*/


    public void PlayUI(string key, float volume = 1f) => Play(key, uiGroup, volume);

    public void PlayBGM(string key)
    {
        if ( bgmSource.isPlaying) 
        {
            bgmSource.Stop();
        }

        AudioClip clip = GetClip(key);

        bgmSource.clip = clip;
        bgmSource.outputAudioMixerGroup = bgmGroup;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public AudioClip GetClip(string key)
    {
        if (clipDictionary.TryGetValue(key, out var clip))
        {
            return clip;
        }
        else
        {
            Debug.LogWarning($"[SoundManager] Clip not found for key: {key}");
            return null;
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
        sp.gameObject.SetActive(false);
        pool.Enqueue(sp);
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
