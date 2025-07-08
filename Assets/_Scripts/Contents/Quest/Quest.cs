using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public QuestInfoSO questInfo;

    public QuestState state;
    public int currentProgress;

    public GameObject questObject;

    public Quest(QuestInfoSO questInfo)
    {
        this.questInfo = questInfo;

        this.state = QuestState.CanStart;
        this.currentProgress = 0;
    }

    public Quest(QuestInfoSO questInfo, QuestState state, int currentProgress)
    {
        this.questInfo = questInfo;
        this.state = state;
        this.currentProgress = currentProgress;
    }

    public void UpdateProgress(int updated = 1)
    {
        currentProgress += updated;
        Manager.Instance.questManager.UpdateQuest(questInfo.id);
    }

    public QuestData GetQuestData(string id)
    {
        return new QuestData(questInfo.id, state, currentProgress);
    }
/*
    public void InstantiateQuest()
    {
        questObject = new GameObject($"{questInfo.questTitle}");
    }*/

}
