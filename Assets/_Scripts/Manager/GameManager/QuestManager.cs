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


    private Dictionary<string, Quest> LoadAllQuests()
    {
        // folder path : Assets/Resources/Quests
        QuestInfoSO[] allQuestsSO = Resources.LoadAll<QuestInfoSO>("Quests");

        Dictionary<string, Quest> idToQuestDict = new Dictionary<string, Quest>();

        foreach (QuestInfoSO so in  allQuestsSO)
        {
            if (!idToQuestDict.ContainsKey(so.id))
            {
                idToQuestDict.Add(so.id, CreateQuestObject(so));
            }
        }
        Debug.Log($"load all quests : {idToQuestDict.Count}");
        return idToQuestDict;
    }

    private Quest CreateQuestObject(QuestInfoSO questInfo)
    {
        Quest quest = null ;
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

    private void OnApplicationQuit()
    {
        foreach (Quest quest in questDict.Values)
        {
            SaveQuest(quest);
        }
    }

    private void SaveQuest(Quest quest)
    {
        QuestData questData = quest.GetQuestData();
        string serializedData = JsonUtility.ToJson(questData);
        PlayerPrefs.SetString(quest.questInfo.id, serializedData);
    }


}
