using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    public string id;
    public QuestState state;
    public int currentProgress;

    public QuestData(string id, QuestState state, int currentProgress)
    {
        this.id = id;
        this.state = state;
        this.currentProgress = currentProgress;
    }
}

[System.Serializable]
public class QuestsData
{
    public QuestData[] questDatas;
    public int questCount = 0;

    public QuestData GetQuestData(string id)
    {
        for (int i = 0; i < questDatas.Length; i++)
        {
            if (questDatas[i] != null && questDatas[i].id == id) return questDatas[i];
        }

        return null;
    }

    public bool PushQuestData(QuestData data)
    {
        // find emty element
        for (int i = 0; i < questCount; i++)
        {
            if (questDatas[i] == null)
            {
                questDatas[i] = data;
                return true;
            }
        }

        // can't find empty element
        Debug.LogWarning($"can't push QuestData {data.id}");
        return false;
    }
}