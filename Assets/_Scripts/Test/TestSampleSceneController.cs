using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 중간평가 이후에 수정돼야 한다.
/// 대화 및 브금 실행 용도의 임시 클래스 
/// </summary>
public class TestSampleSceneController : MonoBehaviour
{
    [SerializeField] NPCDialogueSO guideNPC;
    [SerializeField] TextAsset guideTextAsset;

    ImageUI fadeUI;


    private void Start()
    {
       fadeUI =  Manager.Instance.uiManager.GetUI(UIType.FadeImage).GetComponent<ImageUI>();
        // TODO : 플레이어 입력 방지

        StartCoroutine(DelayedDialogueStart());
    }
    private IEnumerator DelayedDialogueStart()
    {
        fadeUI.HideUI();
        yield return new WaitForSeconds(0.5f/*fadeUI.fadeTime*/);

        DialogueManager.Instance.EnterDialogue(guideTextAsset, null, guideNPC);
    }
}
