using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    [SerializeField] public QuestInfoSO questInfo;

    [SerializeField] public bool forceStart; // for test

    private string id;

    private void Start()
    {
        id = questInfo.id;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"player enetered quest {questInfo.questTitle} zone, this quest is {Manager.Instance.questManager.GetQuestById(questInfo.id).state.ToString()}");

        if (CheckCanStartQuest())
        {
            Manager.Instance.questManager.StartQuest(id);
            Manager.Instance.questManager.ChangeQuestState(id, QuestState.InProgress);
        }
        else
        {
            // when you needs player's acceptance
        }
    }


    private bool CheckCanStartQuest()
    {
         QuestState state = Manager.Instance.questManager.GetQuestState(id);
        Debug.Log(state);
        // started automatically without player interaction or forced
        if ((questInfo.isAutoStart && state == QuestState.CanStart) || (forceStart))
        {
            return true;
        }
        return false;
    }


}
