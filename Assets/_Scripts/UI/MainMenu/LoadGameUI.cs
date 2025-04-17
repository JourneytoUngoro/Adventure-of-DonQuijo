using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGameUI : MonoBehaviour
{
    public Button[] saveSlots;
    public Button[] deleteButtons;

    SaveSlotView[] slotView;
    PopupUI popup;
    Dictionary<string, GameData> allProfilesGameData;

    string profileId = string.Empty;

    private void Awake()
    {
        popup = GetComponent<PopupUI>();

        slotView = new SaveSlotView[saveSlots.Length];
        for (int i = 0; i < saveSlots.Length; i++)
        {
            slotView[i] = saveSlots[i].GetComponent<SaveSlotView>();
            slotView[i].SetOutline(false);
        }
    }
    private void Start()
    {
        popup.SetDynamicPopupEvent(OnClickConfirmButton, OnClickCancelButton);
        InitializeSlots();
    }


    void OnClickConfirmButton()
    {
        if (profileId != string.Empty)
        {
            Manager.Instance.dataManager.ChangeSelectedProfileId(profileId);
            popup.HideUI();
            SceneManager.LoadScene("SampleScene");
        }

    }

    void OnClickCancelButton()
    {
        popup.HideUI();
    }

    public void InitializeSlots()
    {
        // TODO : DataManager로 profileId 연결하기
        allProfilesGameData = Manager.Instance.dataManager.GetAllProfilesGameData();

        int index = 0;
        foreach (var pair in allProfilesGameData)
        {
            Debug.Log("slot init...");

            if (index >= saveSlots.Length) break;

            slotView[index].SetData(pair.Key, pair.Value);
            index++;
        }

    }

    public void OnClickSlotButton(int index)
    {
        for (int i = 0; i < slotView.Length; i++)
        {
            slotView[i].SetOutline(false);
        }
        profileId = slotView[index].profileId;
        slotView[index].SetOutline(true);
    }

    public void OnClickEditButton()
    {
        for (int i = 0; i < deleteButtons.Length; i++)
        {
            deleteButtons[i].gameObject.SetActive(!(deleteButtons[i].gameObject.activeSelf));
        }
    }

    public void OnClickDeleteButton(int index)
    {
        string deleteProfile = slotView[index].profileId;
        Manager.Instance.dataManager.DeleteProfileData(deleteProfile);
        InitializeSlots();
    }



}
