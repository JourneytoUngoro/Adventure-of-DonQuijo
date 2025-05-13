using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

/// <summary>
/// MainMenu에 붙어, 컷씬을 로드한다
/// 임시 함수이고, 중간평가 이후 수정돼야 함 
/// </summary>
public class CutsceneController : MonoBehaviour
{
    public void PlayCutScene()
    {
        SceneManager.LoadScene("FirstCutScene");
        Manager.Instance.soundManager.PlayBGM("cutscene01BGM");
    }

}
