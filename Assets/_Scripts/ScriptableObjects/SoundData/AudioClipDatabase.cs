using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundClip
{
    [HorizontalGroup("Split")]
    public string key;

    [HorizontalGroup("Split")]
    public AudioClip clip;
}


[CreateAssetMenu(fileName = "AudioClipDatabase", menuName = "Sound/AudioClipDatabase")]
public class AudioClipDatabase : ScriptableObject
{
    public SoundClip[] soundClips;

    private Dictionary<string, AudioClip> clipDict;

    public void Init()
    {
        clipDict = new Dictionary<string, AudioClip>();

        foreach (var clip in soundClips)
        {
            if (!clipDict.ContainsKey(clip.key))
            {
                clipDict.Add(clip.key, clip.clip);
            }
            else
            {
                Debug.LogWarning($"Duplicate sound key detected: {clip.key}");
            }
        }
    }

    public AudioClip GetClip(string key)
    {
        if (clipDict == null) Init();

        if (clipDict.TryGetValue(key, out var clip))
            return clip;

        Debug.LogWarning($"Sound key not found: {key}");
        return null;
    }
}
