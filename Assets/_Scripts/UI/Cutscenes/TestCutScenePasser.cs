using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;


/// <summary>
/// FirstCutScene -> SampleScene
/// </summary>
public class TestCutScenePasser : MonoBehaviour
{

    [SerializeField] private TextAsset guideText;
    [SerializeField] private Animator animator;
    [SerializeField] private NPCDialogueSO guideNPC;

    private PlayableDirector director;
    void Start()
    {
        Manager.Instance.uiManager.GetUI(UIType.FadeImage).GetComponent<ImageUI>().HideUI();
        director = GetComponent<PlayableDirector>();
        director.stopped += StartCutSceneEnd;
    }

    private void StartCutSceneEnd(PlayableDirector dir)
    {
        StartCoroutine(CutSceneEnd());
    }

    IEnumerator CutSceneEnd()
    {
        ImageUI fadeUI = Manager.Instance.uiManager.GetUI(UIType.FadeImage).GetComponent<ImageUI>();
        fadeUI.ShowUI();

        yield return new WaitForSeconds(fadeUI.fadeTime);

        SceneManager.LoadScene("SampleScene");
        Manager.Instance.soundManager.PlayBGM("battleBGM");
    }
}