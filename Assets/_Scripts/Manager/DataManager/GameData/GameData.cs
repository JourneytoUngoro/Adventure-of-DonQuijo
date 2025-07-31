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

    // player attributes
    public float currentHealth;
    public float currentDurability;
    public float currentPosture;
    public float currentPower;
    public float currentExperience;
    public float currentAgility;

    // game attributes
    public Vector3 lastPlayerPosition;
    public bool[] bossDefeat;
    public string currentScene;
    // public SerializableDictionary<string, bool> mapOpened
    // public bool[] abilityGained; ��

    public InventoryData inventoryData;
    public ItemUsageData itemUsageData;

    public QuestsData questsData;
    public StageData stageData;
    public CoinData coinData;
    public MemoryFragmentData memoryFragmentData;

    public int mentality;
    public float totalPlayTime;

    #region Not Used Yet 

    // �Ʒ��� ���� �Լ��� �ۼ��ؼ� Guid�� ���� �� �ִ�. ���� ContextMenu�� ���ؼ� ���� ����ų� Start �Ǵ� Awake���� �ڵ����� ����ǵ��� �������.
    /*[ContextMenu("Generate guid for type")]
    private void GenerateGuid()
    {
        type = System.Guid.NewGuid().ToString();
    }
    
    public void LoadInventoryData(GameData data)
    {
        // ���� ������ ����
        data.dictionaryName.TryGetValue(type, out value);
        // do something
        
        // ��� ������ ����
        foreach(KeyValuePair<T1, T2> pair in data.dictionaryName)
        {
            // pair.Key �Ǵ� pair.Value�� ���� Ű�� �����Ϳ� ����
            // do something
        }
    }

    public void SaveInventoryData(ref GameData data)
    {
        if (data.dictionaryName.ContainsKey(type))
        {
            data.dictionaryName.Remove(type);
        }
        data.dictionaryName.Add(type, value);
    }
    */

    #endregion

    public GameData()
    {
        this.displayedLastPlayTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        this.lastPlayTime = DateTime.Now.Ticks;
        this.currentHealth = 100.0f;
        this.currentPosture = 100.0f;
        this.currentPower = 1.0f;
        this.currentDurability = 1.0f;
        this.currentExperience = 0.0f;
        this.currentAgility = 1.0f; // Stats.speedLevel
        this.lastPlayerPosition = new Vector3(-350.0f, -100f, 0.0f);
        this.currentScene = "SampleScene";
        // mapOpened = new Dictionary<string, bool>();

        this.inventoryData = new InventoryData();
        this.itemUsageData = new ItemUsageData();
        this.questsData = new QuestsData();
        this.stageData = new StageData();
        this.coinData = new CoinData();
        this.memoryFragmentData = new MemoryFragmentData();
    }
}