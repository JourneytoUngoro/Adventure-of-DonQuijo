using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGameUI : MonoBehaviour
{
    [Header("Show Cutscene")] [SerializeField] private bool showCutScene;

    [Tooltip("must be assigned in Inspector")]
    public Button[] saveSlotButtons;
    public Button editButton;
    public Sprite editSprtie;
    public Sprite saveSprtie;
    private Image editStateImage;

    public string nowProfileId = string.Empty;
    public string deleteProfileId = string.Empty;

    SaveSlotUI[] saveSlotUI;
    PopupUI loadPopup;
    PopupUI guidePopup;

    Dictionary<string, GameData> allProfilesGameData;

    bool editing;
    bool playOpening;

    private void Awake()
    {
        loadPopup = GetComponent<PopupUI>();
        editStateImage = editButton.GetComponent<Image>();
    }

    private void Start()
    {
        // 게임 로드 패널 버튼에 이벤트 할당
        SetEvents();

        // 저장 슬롯 초기화
        saveSlotUI = new SaveSlotUI[saveSlotButtons.Length];

        for (int i = 0; i < saveSlotButtons.Length; i++)
        {
            saveSlotUI[i] = saveSlotButtons[i].GetComponent<SaveSlotUI>();
            saveSlotUI[i].profileId = GenerateSlotId(i);
        }

        AssginGameData();

        editing = false;
    }

    void SetEvents()
    {
        loadPopup.SetDynamicPopupEvent(null, OnClickCancelButton);

        editButton.onClick.AddListener(OnClickEditButton);
    }

    private bool CheckPlayOpeningWithNewSlot()
    {
        bool skip = PlayerPrefs.GetInt(EnvironmentController.CUTSCENE_SKIP, 0) == 0 ? false : true;

        Debug.Log($"cutscene skip is {skip}");

        return !skip;
    }

    void OnClickCancelButton()
    {
        loadPopup.HideUI();
    }

    public void AssginGameData()
    {
        allProfilesGameData = Manager.Instance.dataManager.GetAllProfilesGameData();

        for (int i = 0; i < saveSlotButtons.Length; i++)
        {
            if (allProfilesGameData.ContainsKey(GenerateSlotId(i)) && allProfilesGameData[GenerateSlotId(i)] != null)
            {
                saveSlotUI[i].SetData(allProfilesGameData[GenerateSlotId(i)]);
            }
            else
            {
                // new game data
                saveSlotUI[i].SetData(null);
            }
        }
    }

    public void OnClickSlotButton(int index)
    {
        LoadOrNewGame(index);
    }

    public void LoadOrNewGame(int index)
    {
        playOpening = CheckPlayOpeningWithNewSlot();

        if (saveSlotUI[index].isNull)
        {
            guidePopup = Manager.Instance.uiManager.ShowDynamicPopup(new PopupData(
                                "", $"슬롯 {index + 1}에서 새 게임을 시작하겠습니까?", "", ""));
            TutorialDialogueController.newGame = true;
        }
        else
        {
            guidePopup = Manager.Instance.uiManager.ShowDynamicPopup(new PopupData(
                                    "", $"슬롯 {index + 1}에서 게임을 불러오겠습니까?", "", ""));
            playOpening = false;
            TutorialDialogueController.newGame = false;
        }

        nowProfileId = saveSlotUI[index].profileId;

        guidePopup.ShowUI();
        guidePopup.SetDynamicPopupEvent(LoadGameWithSlot, () => guidePopup.HideUI());
    }

    public void OnClickEditButton()
    {
        editing = !editing;

        for (int i = 0; i < saveSlotUI.Length; i++)
        {
            if (saveSlotUI[i].isNull) continue;
            saveSlotUI[i].deleteButton.gameObject.SetActive(editing);
        }
        editStateImage.sprite = editing ? saveSprtie : editSprtie;
    }

    public void OnClickDeleteButton(int index)
    {
        deleteProfileId = saveSlotUI[index].profileId;

        guidePopup = Manager.Instance.uiManager.ShowDynamicPopup(new PopupData(
                            "", $"슬롯 {index + 1}에 저장된 데이터를 삭제하겠습니까?", "", ""));
        guidePopup.ShowUI();

        guidePopup.SetDynamicPopupEvent(DeleteGameWithSlot, () => guidePopup.HideUI());
    }

    public void LoadGameWithSlot()
    {
        if (nowProfileId != string.Empty)
        {
            Manager.Instance.dataManager.ChangeSelectedProfileId(nowProfileId);
            guidePopup.HideUI();
            guidePopup = null;

            nowProfileId = string.Empty;

            if (playOpening)
            {
                // TODO : cutscene01 재생으로 바꿔야 한다
                Debug.Log("Play Opening Cutscene");
                Manager.Instance.sceneTransitionManager.SceneTransition(new SceneField("OpeningCutscene"), true, false);
                Manager.Instance.soundManager.PlayBGM("cutscene01BGM");
            }
            else
            {
                Debug.Log("Don't Play Opening Cutscene");
                Manager.Instance.sceneTransitionManager.SceneTransition(new SceneField("Stage1"), true, true);
                Manager.Instance.soundManager.PlayBGM("battleBGM");
            }
        }
        loadPopup.HideUI();
    }

    // IEnumerator LoadCurrentScene()
    // {
    //     yield return null;

    //     // TODO : Save-Load 시 저장된 씬 불러와야 한다 
    //     if (playOpening)
    //     {
    //         // TODO : cutscene01 재생으로 바꿔야 한다
    //         Debug.Log("Play Opening Cutscene");
    //         Manager.Instance.sceneTransitionManager.SceneTransition(new SceneField("OpeningCutscene"), true, false);
    //         Manager.Instance.soundManager.PlayBGM("battleBGM");
    //     }
    //     else
    //     {
    //         Debug.Log("Don't Play Opening Cutscene");
    //         Manager.Instance.sceneTransitionManager.SceneTransition(new SceneField("SampleScene"), true, false);
    //         Manager.Instance.soundManager.PlayBGM("battleBGM");
    //     }
    // }

    void DeleteGameWithSlot()
    {
        if (deleteProfileId != string.Empty)
        {
            Manager.Instance.dataManager.DeleteProfileData(deleteProfileId);
            guidePopup.HideUI();
            guidePopup = null;

            OnDeletedSaveSlot();
            AssginGameData();

            deleteProfileId = string.Empty;
        }
    }

    void OnDeletedSaveSlot()
    {
        for (int i = 0; i < saveSlotUI.Length; i++)
        {
            if (saveSlotUI[i].profileId == deleteProfileId)
            {
                saveSlotUI[i].SetData(null);
                if (editing) saveSlotUI[i].deleteButton.gameObject.SetActive(false);
                break;
            }
        }
    }

    string GenerateSlotId(int index)
    {
        string slotId = "slot";
        // profileId : slot1, slot2, slot3, ...
        return string.Concat(slotId, index+1);
    }

    public void ShowLoadGamePanel()
    {
        if (editing)
        {
            OnClickEditButton();
        }
    }
}
