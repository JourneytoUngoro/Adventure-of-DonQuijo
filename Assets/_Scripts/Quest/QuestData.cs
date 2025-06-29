using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    public QuestState state;
    public int nowProgress;

    public QuestData(QuestState state, int nowProgress)
    {
        this.state = state;
        this.nowProgress = nowProgress;
    }

}
