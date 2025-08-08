using Ink.Parsed;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialDialogueController : MonoBehaviour
{
    [Title("Animator")] [SerializeField] private Animator animator;
    [Title("Ink JSON")] [SerializeField] private TextAsset inkJSON;
    [Title("PortraitSO")] [SerializeField] private NPCDialogueSO npcSO;
    [Title("Dialogue Panel")] [SerializeField] GameObject dialoguePanel;

    private bool skipTutorial;
    public static bool newGame;
    public static bool tutorialPlayed;
    private bool shouldPlay;
    private float watiTime = 0.06f;

    private Transform originalParent;

    private void Awake()
    {
        skipTutorial = PlayerPrefs.GetInt(EnvironmentController.TUTORIAL_SKIP, 0) == 0 ? false : true;

        if (!skipTutorial && newGame && !tutorialPlayed)
        {
            shouldPlay = true;
            originalParent = transform.parent;

            StartCoroutine(PlayTutorialAfterWait());
        }
        else
        {
            dialoguePanel.SetActive(false);
            shouldPlay = false;
        }
    }

    IEnumerator PlayTutorialAfterWait()
    {
        Manager.Instance.inputHandler.SetCharacterControlEnabled(false);

        yield return new WaitForSeconds(watiTime);
        tutorialPlayed = true;
        PlayTutorialDialogue();
    }

    private void PlayTutorialDialogue()
    {
        // 대화 도중 설정 등의 패널 활성화 시 대화 위에 그려지도록 하기 위해 
        dialoguePanel.transform.SetParent(Manager.Instance.uiManager.GetUICanvasTransfrom(), false);
        dialoguePanel.transform.SetSiblingIndex(dialoguePanel.transform.parent.childCount - 1);

        DialogueManager.Instance.SetOnExitDialogue(OnExitTutorial);

        dialoguePanel.GetComponent<CanvasGroup>().alpha = 1f;
        Debug.Log("Play Tutorial!" + !skipTutorial);
        DialogueManager.Instance.EnterDialogue(inkJSON, animator, npcSO);
    }

    private void OnExitTutorial()
    {
        if (originalParent == null)
        {
            originalParent = GameObject.Find("Map1_Canvas").transform;
        }

        dialoguePanel.transform.SetParent(originalParent);
    }

    private void OnDestroy()
    {
        if (dialoguePanel != null)
        {
            Destroy(dialoguePanel);
        }
    }
}
