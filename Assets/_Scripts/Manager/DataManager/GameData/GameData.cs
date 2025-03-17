using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Must Use 'JSON .NET For Unity' in Unity Asset store for storing complex data types like Dictionary etc.
// Or make your own Dictionary using serializable data types such as List

[System.Serializable]
public class GameData
{
    public string displayedLastPlayTime;
    public long lastPlayTime;
    public int playerLevel;
    public int statPoints;
    public int attackLevel;
    // 공격력, 에너지 관련
    public int enduranceLevel;
    // 최대 체력 및 방어력
    public int postureLevel;
    // 체간
    public int dexterityLevel;
    // 민첩
    public Vector3 lastSavePosition;
    public string lastSavePoint;
    public bool[] bossDefeat;
    public string currentScene;
    // public SerializableDictionary<string, bool> mapOpened
    // public bool[] abilityGained; 등
    // 아래와 같은 함수를 작성해서 Guid를 만들 수 있다. 가령 ContextMenu를 통해서 직접 만들거나 Start 또는 Awake에서 자동으로 실행되도록 만들던가.
    /*[ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
    
    public void LoadData(GameData data)
    {
        // 개별 데이터 접근
        data.dictionaryName.TryGetValue(id, out value);
        // do something
        
        // 모든 데이터 접근
        foreach(KeyValuePair<T1, T2> pair in data.dictionaryName)
        {
            // pair.Key 또는 pair.Value를 통해 키와 데이터에 접근
            // do something
        }
    }

    public void SaveData(ref GameData data)
    {
        if (data.dictionaryName.ContainsKey(id))
        {
            data.dictionaryName.Remove(id);
        }
        data.dictionaryName.Add(id, value);
    }
    */

    public GameData()
    {
        this.displayedLastPlayTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.lastPlayTime = DateTime.Now.Ticks;
        this.playerLevel = 1;
        this.statPoints = 0;
        this.attackLevel = 1;
        this.enduranceLevel = 1;
        this.postureLevel = 1;
        this.dexterityLevel = 1;
        this.lastSavePosition = Vector2.zero;
        this.lastSavePoint = "";
        this.currentScene = "SampleScene";
        // mapOpened = new Dictionary<string, bool>();
    }
}