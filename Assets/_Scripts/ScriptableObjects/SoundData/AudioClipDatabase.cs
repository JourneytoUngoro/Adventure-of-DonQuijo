using UnityEngine;


[CreateAssetMenu(fileName = "AudioClipDatabase", menuName = "Sound/AudioClipDatabase")]
public class AudioClipDatabase : ScriptableObject
{
    public AudioClip[] clips;
}
