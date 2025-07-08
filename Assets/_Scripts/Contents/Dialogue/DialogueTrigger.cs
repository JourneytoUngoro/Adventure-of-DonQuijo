using UnityEngine;
using Sirenix.OdinInspector;


public class DialogueTriggerUpdated : MonoBehaviour
{
    [Title("Visual Que")][SerializeField] private GameObject visualQue;
    [Title("Animator")][SerializeField] private Animator animator;
    [Title("Ink JSON")][SerializeField] private TextAsset inkJSON;
    [Title("Player LayerMask")][SerializeField] private LayerMask player;
    [Title("PortraitSO")][SerializeField] private NPCDialogueSO npcSO;
    private bool playerInRange;


    private void Start()
    {
        playerInRange = false;
        visualQue.SetActive(false);
    }
    private bool PressedNextLineKey()
    {
        return Input.GetKeyDown(KeyCode.I);
    }

    private void Update()
    {
        if (playerInRange && !DialogueManager.Instance.isDialoguePlaying)
        {
            visualQue.SetActive(true);
            if (PressedNextLineKey())
            {
                DialogueManager.Instance.EnterDialogue(inkJSON, animator, npcSO);
            }
        }
        else
        {
            visualQue.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & player.value) != 0)
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & player.value) != 0)
        {
            playerInRange = false;
        }
    }
}
