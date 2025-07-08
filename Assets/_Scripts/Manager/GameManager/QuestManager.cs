using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour, IDataPersistance
{
    [ShowInInspector]
    private Dictionary<string, Quest> questDict;

    public int totalQuestsCount { get; private set; }

    private QuestsData questsData = null;

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
        if (quest.currentProgress >= quest.questInfo.goalProgress)
        {
            Debug.Log($"{quest.currentProgress} >= {quest.questInfo.goalProgress}");
            return true;
        }
        return false;
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

            // quest completed automatically without player interactoin
            if (quest.questInfo.isAutoComplete)
            {
                CompleteQuest(id);
            }
        }
    }

    public void CompleteQuest(string id)
    {
        // CheckState.CanComplete -> CheckState.Completed
        if (!(GetQuestState(id) == QuestState.CanComplete)) return;
        ChangeQuestState(id, QuestState.Completed);
        RequestReward(id);
    }

    public void RequestReward(string id)
    {
        Debug.Log("try give reward");
        Reward reward = GetQuestById(id).questInfo.reward.GetComponent<Reward>();
        
        if (!reward.canGiveReward) return;
        reward.GiveRewardTo();
    }

    public QuestState GetQuestState(string id) => GetQuestById(id).state;

    private Dictionary<string, Quest> LoadAllQuests()
    {
        // folder path : Assets/Resources/Quests
        QuestInfoSO[] allQuestsSO = Resources.LoadAll<QuestInfoSO>("Quests");

        Dictionary<string, Quest> idToQuestDict = new Dictionary<string, Quest>();

        foreach (QuestInfoSO so in allQuestsSO)
        {
            if (!idToQuestDict.ContainsKey(so.id))
            {
                idToQuestDict.Add(so.id, new Quest(so));
            }
        }
        totalQuestsCount = idToQuestDict.Count;
        
        return idToQuestDict;
    }

    private void SaveQuest(Quest quest)
    {
        QuestData questData = quest.GetQuestData(quest.questInfo.id);
        string serializedData = JsonUtility.ToJson(questData);
        PlayerPrefs.SetString(quest.questInfo.id, serializedData);
    }

    private void OnApplicationQuit()
    {
        foreach (Quest quest in questDict.Values)
        {
            SaveQuest(quest);
        }
    }

    public void LoadData(GameData data)
    {
        questDict = LoadAllQuests();

        questsData = data.questsData;

        bool isNew = questsData.questDatas == null || questsData.questCount == 0;

        if (isNew)
        {
            questsData.questCount = QuestsDatabase.totalQuests;
            questsData.questDatas = new QuestData[questsData.questCount];

            // Debug.Log("new game started, and new quests data created");
            return;
        }

        foreach (Quest quest in questDict.Values)
        {
            QuestData questData = questsData.GetQuestData(quest.questInfo.id);
            quest.currentProgress = questData.currentProgress;
            quest.state = questData.state;
        }
    }

    public void SaveData(GameData data)
    {
        QuestsData quests = new QuestsData();
        quests.questCount = QuestsDatabase.totalQuests;
        quests.questDatas = new QuestData[quests.questCount];

        foreach (Quest quest in questDict.Values)
        {
            QuestData questData = quest.GetQuestData(quest.questInfo.id);
            quests.PushQuestData(questData);
        }
        data.questsData = quests;
    }
}