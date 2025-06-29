using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [ShowInInspector]
    private Dictionary<string, Quest> questDict;

    private void Awake()
    {
        questDict = LoadAllQuests();
    }

    private Quest CreateQuestObject(QuestInfoSO questInfo)
    {
        Quest quest = null;
        if (PlayerPrefs.HasKey(questInfo.id))
        {
            string serializedData = PlayerPrefs.GetString(questInfo.id);
            QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
            quest = new Quest(questInfo, questData.state, questData.nowProgress);
        }
        else
        {
            quest = new Quest(questInfo);
        }

        Debug.Assert(quest != null, "can't load quest info!");

        return quest;
    }

    public Quest GetQuestById(string id)
    {
        Quest quest = questDict[id];
        Debug.Assert(quest != null, $"can't find quest with id \'{id}\'");

        return quest;
    }

    public void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);

        if (quest == null)
        {
            Debug.LogWarning("wrong quest accessed");
        }

        quest.state = state;
    }

    public bool CheckClearQuest(Quest quest)
    {
        if (quest.currentProgress >= quest.goalProgress)
        {
            Debug.Log($"clear {quest.questInfo.questTitle} quest!");
            return true;
        }

        return false;
    }

    private void OnApplicationQuit()
    {
        foreach (Quest quest in questDict.Values)
        {
            SaveQuest(quest);
        }
    }

    public void StartQuest(string id)
    {
        Quest quest = GetQuestById(id);

        ChangeQuestState(id, QuestState.InProgress);
    }

    public void UpdateQuest(string id)
    {
        Quest quest = GetQuestById(id);
        if(CheckClearQuest(quest))
        {
            ChangeQuestState(id, QuestState.CanComplete);
        }
        else
        {

        }
    }

    public void FinishQuest(string id)
    {
        Quest quest = GetQuestById(id);
        


    }
    private Dictionary<string, Quest> LoadAllQuests()
    {
        // folder path : Assets/Resources/Quests
        QuestInfoSO[] allQuestsSO = Resources.LoadAll<QuestInfoSO>("Quests");

        Dictionary<string, Quest> idToQuestDict = new Dictionary<string, Quest>();

        foreach (QuestInfoSO so in allQuestsSO)
        {
            if (!idToQuestDict.ContainsKey(so.id))
            {
                idToQuestDict.Add(so.id, CreateQuestObject(so));
            }
        }
        Debug.Log($"load all quests : {idToQuestDict.Count}");
        return idToQuestDict;
    }


    private void SaveQuest(Quest quest)
    {
        QuestData questData = quest.GetQuestData();
        string serializedData = JsonUtility.ToJson(questData);
        PlayerPrefs.SetString(quest.questInfo.id, serializedData);
    }

    private void RequestReward(Quest quest)
    {
        // add get reward
    }

}