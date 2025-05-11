using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Test_DialogueTrigger : MonoBehaviour
{
    public TextAsset inkJSON;
    public Animator animator;
    public NPCDialogueSO npcSO;

    public void OnClickButton()
    {
        if (!DialogueManager.Instance.isDialoguePlaying)
        {
            DialogueManager.Instance.EnterDialogue(inkJSON, animator, npcSO);
        }
    }
}
