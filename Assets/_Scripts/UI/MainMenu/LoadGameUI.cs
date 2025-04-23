using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGameUI : MonoBehaviour
{
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

    private void Awake()
    {
        popup = GetComponent<PopupUI>();
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
            saveSlotUI[i].SetOutline(false);
            saveSlotUI[i].profileId = GenerateSlotId(i);
        }

        AssginGameData();

        editing = false;
    }

    void SetEvents()
    {
        popup.SetDynamicPopupEvent(null, OnClickCancelButton);

        editButton.onClick.AddListener(OnClickEditButton);
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
        for (int i = 0; i < saveSlotUI.Length; i++)
        {
            saveSlotUI[i].SetOutline(false);
        }
        saveSlotUI[index].SetOutline(true);

        LoadOrNewGame(index);
    }

    public void LoadOrNewGame(int index)
    {

        if (saveSlotUI[index].isNull)
        {
            guidePopup = Manager.Instance.uiManager.ShowDynamicPopup(new PopupData(
                                "=== New Game ===", $"New Game with Slot {index + 1}?", "Yes", "No"));
        }
        else
        {
            guidePopup = Manager.Instance.uiManager.ShowDynamicPopup(new PopupData(
                                    "=== Load Game ===", $"Load Game with Slot {index + 1}?", "Yes", "No"));
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
        editButton.GetComponentInChildren<TextMeshProUGUI>().text = editing ? "Editing..." : "Edit";
    }

    public void OnClickDeleteButton(int index)
    {
        deleteProfileId = saveSlotUI[index].profileId;

        guidePopup = Manager.Instance.uiManager.ShowDynamicPopup(new PopupData(
                            "=== Delete Slot ===", $"Delete Slot {index + 1}?", "Yes", "No"));
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

            LoadCurrentScene();
        }
        popup.HideUI();
    }

    void LoadCurrentScene()
    {
        // TODO : Save-Load 시 저장된 씬 불러와야 한다 
        SceneManager.LoadScene("SampleScene");
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
