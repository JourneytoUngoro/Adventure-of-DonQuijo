using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundPlayer : MonoBehaviour
{
    public bool isPlaying;
    public bool canPlaying;

    private AudioSource audioSource;
    private System.Action<SoundPlayer> onFinished;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        isPlaying = false;
        canPlaying = true;
    }

    public void Play(AudioClip clip, AudioMixerGroup mixer, float volume, System.Action<SoundPlayer> onFinished)
    {
        this.onFinished = onFinished;
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.outputAudioMixerGroup = mixer;

        isPlaying = true;
        audioSource.Play();

        Invoke(nameof(ReturnToPool), clip.length);
        isPlaying = false;
    }

    public void Play(AudioClip clip, AudioMixerGroup mixer, float volume, System.Action<SoundPlayer> onFinished, Transform spawnsTransform, float pitch)
    {
        this.onFinished = onFinished;
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.transform.position = spawnsTransform.position;
        audioSource.pitch = pitch;
        audioSource.outputAudioMixerGroup = mixer;

        isPlaying = true;
        audioSource.Play();

        Invoke(nameof(ReturnToPool), clip.length);
    }

    private void ReturnToPool()
    {
        onFinished?.Invoke(this);
    }

    public void StopPlayingAudioClip()
    {
        audioSource.Stop();
    }

}

