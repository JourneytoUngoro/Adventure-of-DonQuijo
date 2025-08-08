using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class StageManager : MonoBehaviour, IDataPersistance
{
    private Dictionary<string, Stage> stageDict;
    public TextMeshProUGUI mfTMP; // 인스펙터 할당 
    public Stage nowStage {  get; private set; }

    private void Awake()
    {
        LoadAllStages();

        Debug.Log(Manager.Instance.gameManager == null ? "GameManager is null" : "GameManager is not null");

        Manager.Instance.gameManager.sceneClearedAction -= DisplayStageClear;
        Manager.Instance.gameManager.sceneClearedAction += DisplayStageClear;
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
            mfTMP.text = "0";
        }
        else
        {
            nowStage = GetStageById(data.stageData.id);
            nowStage.SetData(stageData);
            nowStage.Initialize();

            mfTMP.text = nowStage.collectedMemoryFragmentCount.ToString();
        }
        // Debug.Log($"loaded stage {nowStage.stageInfo.stageName}");
    }

    public void SaveData(GameData data)
    {
        if (nowStage != null && nowStage.GetStageData() != null) 
            data.stageData = nowStage.GetStageData();
    }

    public void DisplayStageClear()
    {
        if (!Manager.Instance.sceneTransitionManager.currentActiveScene.SceneName.Equals("Stage4"))
        {
            Debug.Log("Not Last Stage");
            return;
        }
        else
        {
            Debug.Log("Last Stage Cleared");
        }

        PopupUI stageClearPopup = Object.Instantiate(Resources.Load<GameObject>("Prefabs/UI/Stage Clear")).GetComponent<PopupUI>();
        Debug.Assert(stageClearPopup != null, "can't find Stage Clear Popup");
        stageClearPopup.transform.SetParent(Manager.Instance.uiManager.GetUICanvasTransfrom(), false);
        stageClearPopup.ShowUI();
        stageClearPopup.gameObject.GetComponent<GameClearController>().ShowPlayInfo();

        Manager.Instance.soundManager.PlaySoundFXClip("gameWinSFX", transform);
    }

    public void OnEnemyDeath(Transform enemy) => nowStage.OnEnemyDeath(enemy);

    public void UpdateMFTMP(int count)
    {
        mfTMP.text = count.ToString();
    }
}
