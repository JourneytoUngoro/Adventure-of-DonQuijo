using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    // Variable names in Scripts            // Object names in Scene Hierarchy
    public Transform buttonsTransform;  // Choice Buttons

    private Button continueButton;       // Continue Button
    private Button settingPopupButton; // Setting Popup Button
    private Button saveButton;            // Save Button

    private PopupUI pauseMenuPopup;

    private void Start()
    {
        InitializeReferences();
        RegisterAcivatedEvent();
        RegisterButtonEvents();
    }


    private void InitializeReferences()
    {
        pauseMenuPopup = GetComponent<PopupUI>();

        if (buttonsTransform == null)
        {
            buttonsTransform = transform.Find("Choice Buttons").GetComponent<Transform>();
        }

        continueButton = buttonsTransform.Find("Continue Button").GetComponent<Button>();
        settingPopupButton = buttonsTransform.Find("Setting Popup Button").GetComponent<Button>();
        saveButton = buttonsTransform.Find("Save Button").GetComponent<Button>();
    }

    private void RegisterAcivatedEvent()
    {
        pauseMenuPopup.SetOnShow(OnPauseMenuActivated);
        pauseMenuPopup.SetOnHide(OnPaueMenuInactivated);
    }

    private void OnPauseMenuActivated() => Manager.Instance.gameManager.PauseGame();
    private void OnPaueMenuInactivated() => Manager.Instance.gameManager.ResumeGame();

    private void RegisterButtonEvents()
    {
        continueButton.onClick.RemoveAllListeners();
        settingPopupButton.onClick.RemoveAllListeners();
        saveButton.onClick.RemoveAllListeners();

        continueButton.onClick.AddListener(OnClickContinueButton);
        settingPopupButton.onClick.AddListener(OnClickSettingPopupButton);
        saveButton.onClick.AddListener(OnClickSaveButton);
    }

    private void OnClickContinueButton()
    {
        pauseMenuPopup.HideUI();
    }

    private void OnClickSettingPopupButton()
    {
        Manager.Instance.uiManager.GetUI(UIType.settingPopup).ShowUI();
    }

    private void OnClickSaveButton()
    {
        Manager.Instance.dataManager.SaveGame();
        // TODO : 저장 잘 된 경우를 알아내는 로직 추가 
/*        // if (success)
        {
            Manager.Instance.uiManager.ShowDynamicTextInfo( new TextInfoData("성공적으로 저장됐습니다."));
        }
        // else 
        {
            Manager.Instance.uiManager.ShowDynamicTextInfo(new TextInfoData("저장에 실패했습니다."));
        }*/
    }

}