using Ink.Parsed;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDialogueController : MonoBehaviour
{
    [Title("Animator")] [SerializeField] private Animator animator;
    [Title("Ink JSON")] [SerializeField] private TextAsset inkJSON;
    [Title("PortraitSO")] [SerializeField] private NPCDialogueSO npcSO;
    [Title("Dialogue Panel")] [SerializeField] GameObject dialoguePanel;

    private bool skipTutorial;
    public static bool newGame;
    private float watiTime = 0.3f;

    private void Awake()
    {
        skipTutorial = PlayerPrefs.GetInt(EnvironmentController.TUTORIAL_SKIP, 0) == 0 ? false : true;

        if (!skipTutorial && newGame) StartCoroutine(PlayTutorialAfterWait());
         else dialoguePanel.SetActive(false);
    }

    IEnumerator PlayTutorialAfterWait()
    {
        yield return new WaitForSeconds(watiTime);

        PlayTutorialDialogue();
    }

    private void PlayTutorialDialogue()
    {
        dialoguePanel.GetComponent<CanvasGroup>().alpha = 1f;
        Debug.Log("Play Tutorial!" + !skipTutorial);
        DialogueManager.Instance.EnterDialogue(inkJSON, animator, npcSO);
    }
}
