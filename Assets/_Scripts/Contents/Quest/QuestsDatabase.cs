using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestsDatabase
{
    static Dictionary<string, QuestInfoSO> questsDictionary;

    public static int totalQuests;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void Initialize()
    {
        questsDictionary = new Dictionary<string, QuestInfoSO>();

        var quests = Resources.LoadAll<QuestInfoSO>("Quests");
        foreach (var quest in quests)
        {
            questsDictionary.Add(quest.id, quest);
        }
        totalQuests = questsDictionary.Count;
    }

    public static QuestInfoSO GetDetailsById(string id)
    {
        try
        {
            Debug.Assert(questsDictionary[id] != null, $"DB에 존재하지 않는 퀘스트 {id}");
            return questsDictionary[id];
        }
        catch
        {
            Debug.LogError($"Cannot find quest with : {id}");
            return null;
        }
    }
}
