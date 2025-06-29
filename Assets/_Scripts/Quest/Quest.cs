using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public QuestInfoSO questInfo;

    public QuestState state;
    public int currentProgress;
    public int goalProgress;

    public Quest(QuestInfoSO questInfo)
    {
        this.questInfo = questInfo;

        this.state = QuestState.NotClearedPreQuests;
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
    }

    public QuestData GetQuestData()
    {
        return new QuestData(state, currentProgress);
    }
}
