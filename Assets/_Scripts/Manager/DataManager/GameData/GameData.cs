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
    public int currentLevel;
    public float currentHealth;
    public float currentPosture;
    public float currentExperience;
    public Vector3 lastPlayerPosition;
    public bool[] bossDefeat;
    public string currentScene;
    // public SerializableDictionary<string, bool> mapOpened
    // public bool[] abilityGained; ��

    public InventoryData inventoryData;
    public ItemUsageData itemUsageData;

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
        this.displayedLastPlayTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.lastPlayTime = DateTime.Now.Ticks;
        this.currentLevel = 1;
        this.currentHealth = 100.0f;
        this.currentPosture = 0.0f;
        this.currentExperience = 0.0f;
        this.lastPlayerPosition = Vector3.zero;
        this.currentScene = "SampleScene";
        // mapOpened = new Dictionary<string, bool>();

        this.inventoryData = new InventoryData();
        this.itemUsageData = new ItemUsageData();
    }
}