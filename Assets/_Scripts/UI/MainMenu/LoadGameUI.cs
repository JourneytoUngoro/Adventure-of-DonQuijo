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
        }
        InitializeSlots();

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

    public void InitializeSlots()
    {
        // TODO : DataManager로 nowProfileId 연결하기
        allProfilesGameData = Manager.Instance.dataManager.GetAllProfilesGameData();

        int index = 0;
        foreach (var pair in allProfilesGameData)
        {
            Debug.Log("slot init...");

            if (index >= saveSlotButtons.Length) break;

            saveSlotUI[index++].SetData(pair.Key, pair.Value);
        }

        for (int i = 0; i < saveSlotButtons.Length; i++)
        {
            if (saveSlotUI[i].isNull)
            {
                saveSlotUI[i].SetData(null, null);

            }
        }
    }

    public void OnClickSlotButton(int index)
    {
        for (int i = 0; i < saveSlotUI.Length; i++)
        {
            saveSlotUI[i].SetOutline(false);
        }
        nowProfileId = saveSlotUI[index].profileId;
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

    void LoadGameWithSlot()
    {
        if (nowProfileId != string.Empty)
        {
            Manager.Instance.dataManager.ChangeSelectedProfileId(nowProfileId);
            guidePopup?.HideUI();
            guidePopup = null;

            nowProfileId = string.Empty;

            SceneManager.LoadScene("SampleScene");
        }
    }

    void DeleteGameWithSlot()
    {
        if (deleteProfileId != string.Empty)
        {
            Manager.Instance.dataManager.DeleteProfileData(deleteProfileId);
            guidePopup?.HideUI();
            guidePopup = null;

            OnDeletedSaveSlot();
            InitializeSlots();

            deleteProfileId = string.Empty;
        }
    }

    void OnDeletedSaveSlot()
    {
        for (int i = 0; i < saveSlotUI.Length; i++)
        {
            if (saveSlotUI[i].profileId == deleteProfileId)
            {
                saveSlotUI[i].SetData(null, null);
                if (editing) saveSlotUI[i].deleteButton.gameObject.SetActive(false);
                break;
            }
        }
    }

}
