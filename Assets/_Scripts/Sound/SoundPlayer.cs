using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    private System.Action<SoundPlayer> onFinished;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void Play(AudioClip clip, AudioMixerGroup mixer, float volume, System.Action<SoundPlayer> onFinished)
    {
        this.onFinished = onFinished;
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();

        Invoke(nameof(ReturnToPool), clip.length);
    }

    private void ReturnToPool()
    {
        onFinished?.Invoke(this);
    }

}

