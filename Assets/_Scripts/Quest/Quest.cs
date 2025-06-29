using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public QuestInfoSO questInfo;

    public QuestState state;
    public int nowProgress;

    public Quest(QuestInfoSO questInfo)
    {
        this.questInfo = questInfo;

        this.state = QuestState.NotClearedPreQuests;
        this.nowProgress = 0;
    }

    public Quest(QuestInfoSO questInfo, QuestState state, int nowProgress)
    {
        this.questInfo = questInfo;
        this.state = state;
        this.nowProgress = nowProgress;
    }

    public void UpdateProgress(int updated = 1)
    {
        nowProgress += updated;
    }

    public QuestData GetQuestData()
    {
        return new QuestData(state, nowProgress);
    }
}
