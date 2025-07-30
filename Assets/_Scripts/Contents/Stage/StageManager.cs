using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour, IDataPersistance
{
    private Dictionary<string, Stage> stageDict;

    public Stage nowStage {  get; private set; }

    private void Awake()
    {
        LoadAllStages();
    }

    private void LoadAllStages()
    {
        // folder path : Assets/Resources/Stages
        StageInfoSO[] stageInfos = Resources.LoadAll<StageInfoSO>("Stages");

        stageDict = new Dictionary<string, Stage>();
        foreach (StageInfoSO stageInfo in stageInfos)
        {
            if (!stageDict.ContainsKey(stageInfo.id))
            {
                stageDict.Add(stageInfo.id, new Stage(stageInfo));
            }
        }

        Debug.Log($"load all stages, Count : {stageDict.Count}");
    }

    public void ChangeStage(string id)
    {
        nowStage = GetStageById(id);
        Debug.Log($"changed stage to {nowStage.stageName}");
    }
    
    public Stage GetStageById(string id)
    {
        Stage stage =  stageDict[id];

        return stage;
    }

    public void LoadData(GameData data)
    {
        StageData stageData = data.stageData;

        bool isNew = stageData.id == string.Empty;

        // when starting this stage with new save slot
        if (isNew)
        {
            // Debug.Log("is new ");
            // Considier adding logic to load the first stage automatically
            nowStage = stageDict["FirstStage"];            
        }
        else
        {
            nowStage = GetStageById(data.stageData.id);
            nowStage.SetData(stageData);
            nowStage.Initialize();
        }
        // Debug.Log($"loaded stage {nowStage.stageInfo.stageName}");
    }

    public void SaveData(GameData data)
    {
        if (nowStage != null && nowStage.GetStageData() != null) 
            data.stageData = nowStage.GetStageData();
    }

    public void OnEnemyDeath(Transform enemy) => nowStage.OnEnemyDeath(enemy);
}
