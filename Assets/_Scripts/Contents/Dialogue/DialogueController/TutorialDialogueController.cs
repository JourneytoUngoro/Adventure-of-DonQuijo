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
    private bool newGame;

    private void Awake()
    {
        skipTutorial = PlayerPrefs.GetInt(EnvironmentController.TUTORIAL_SKIP, 0) == 0 ? false : true;
        newGame = true; // TODO : 바꿔야 한다

        if (/*skipTutorial && newGame*/ true) PlayTutorialDialogue();
        // else Destroy(gameObject);
    }

    private void PlayTutorialDialogue()
    {
        Debug.Log("Play Tutorial!" + !skipTutorial);
        dialoguePanel.SetActive(true);
        DialogueManager.Instance.EnterDialogue(inkJSON, animator, npcSO);
    }

}
