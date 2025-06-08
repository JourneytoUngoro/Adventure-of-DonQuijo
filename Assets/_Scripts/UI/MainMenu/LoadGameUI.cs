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

    public string nowProfileId = string.Empty;
    public string deleteProfileId = string.Empty;

    SaveSlotUI[] saveSlotUI;
    PopupUI popup;
    PopupUI guidePopup;

    Dictionary<string, GameData> allProfilesGameData;

    bool editing;
    bool isFirstPlay;

    private void Awake()
    {
        popup = GetComponent<PopupUI>();
        isFirstPlay = false;
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
/*            saveSlotUI[i].SetOutline(false);
*/            saveSlotUI[i].profileId = GenerateSlotId(i);
        }

        AssginGameData();

        editing = false;
        isFirstPlay = CheckFirstPlay();
    }

    void SetEvents()
    {
        popup.SetDynamicPopupEvent(null, OnClickCancelButton);

        editButton.onClick.AddListener(OnClickEditButton);
    }

    private bool CheckFirstPlay()
    {
        return Manager.Instance.dataManager.AllProfilesCount() > 0 ? false : true;
    }

    void OnClickCancelButton()
    {
        popup.HideUI();
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
/*        for (int i = 0; i < saveSlotUI.Length; i++)
        {
            saveSlotUI[i].SetOutline(false);
        }
        saveSlotUI[index].SetOutline(true);*/

        LoadOrNewGame(index);
    }

    public void LoadOrNewGame(int index)
    {

        if (saveSlotUI[index].isNull)
        {
            guidePopup = Manager.Instance.uiManager.ShowDynamicPopup(new PopupData(
                                "", $"슬롯 {index + 1}에서 새 게임을 시작하겠습니까?", "확인", ""));
        }
        else
        {
            guidePopup = Manager.Instance.uiManager.ShowDynamicPopup(new PopupData(
                                    "", $"슬롯 {index + 1}에서 게임을 불러오겠습니까?", "확인", ""));
        }

        nowProfileId = saveSlotUI[index].profileId;

        guidePopup.ShowUI();
        guidePopup.SetDynamicPopupEvent(LoadGameWithSlot, guidePopup.HideUI);
    }

    public void OnClickEditButton()
    {
        editing = !editing;

        for (int i = 0; i < saveSlotUI.Length; i++)
        {
            if (saveSlotUI[i].isNull) continue;
            saveSlotUI[i].deleteButton.gameObject.SetActive(editing);
        }
        editButton.GetComponentInChildren<TextMeshProUGUI>().text = editing ? "저장하기" : "편집하기";
    }

    public void OnClickDeleteButton(int index)
    {
        deleteProfileId = saveSlotUI[index].profileId;

        guidePopup = Manager.Instance.uiManager.ShowDynamicPopup(new PopupData(
                            "", $"슬롯 {index + 1}에 저장된 데이터를 삭제하겠습니까?", "확인", ""));
        guidePopup.ShowUI();

        guidePopup.SetDynamicPopupEvent(DeleteGameWithSlot, guidePopup.HideUI);
    }


    public void LoadGameWithSlot()
    {
        if (nowProfileId != string.Empty)
        {
            Manager.Instance.dataManager.ChangeSelectedProfileId(nowProfileId);
            guidePopup.HideUI();
            guidePopup = null;

            nowProfileId = string.Empty;

            StartCoroutine(LoadCurrentScene());
        }
        popup.HideUI();
    }

    IEnumerator LoadCurrentScene()
    {
        ImageUI fadeUI = Manager.Instance.uiManager.GetUI(UIType.FadeImage).GetComponent<ImageUI>();
        Debug.Assert(fadeUI != null, "fade image is null! ");
        fadeUI.ShowAndHideUI(3f);

        yield return new WaitForSeconds(2.5f);

        // TODO : Save-Load 시 저장된 씬 불러와야 한다 
        if (isFirstPlay)
        {
            // TODO : cutscene01 재생으로 바꿔야 한다
            SceneManager.LoadScene("SampleScene");
            Manager.Instance.soundManager.PlayBGM("battleBGM");
        }
        else
        {
            SceneManager.LoadScene("SampleScene");
            Manager.Instance.soundManager.PlayBGM("battleBGM");
        }
    }

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
