using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Soundmanager : MonoBehaviour
{

    public string dictKey;
    public void PlaySFX()
    {
        SoundManager.Instance.PlaySoundFXClip(dictKey, transform);
    }

    public void PlayBGM()
    {
        SoundManager.Instance.PlayBGM(dictKey); //  
    }
    public void PlayUI()
    {
        SoundManager.Instance.PlayUI(dictKey); //  
    }
}
