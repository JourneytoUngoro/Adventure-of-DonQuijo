using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameUI : MonoBehaviour
{
    [SerializeField] GameObject loadGamePanel;

    PopupUI popup;
    public PopupUI guidePopup;

    private void Awake()
    {
        popup = GetComponent<PopupUI>();
    }

    private void Start()
    {
        popup.ClearDynamicPopupEvent();
        guidePopup.ClearDynamicPopupEvent();

        popup.SetDynamicPopupEvent(OnClickConfirmButton, OnClickCancelButton);
        guidePopup.SetDynamicPopupEvent(OnClickGuideConfrimButton, OnClickGuideCancelButton);
    }

    void OnClickConfirmButton()
    {
        if (Manager.Instance.dataManager.AllProfilesCount() < 3)
        {
            popup.HideUI();
            loadGamePanel.GetComponent<PopupUI>().ShowUI();
        }
        else
        {
            guidePopup.ShowUI();
        }
    }

    void OnClickCancelButton()
    {
        popup.HideUI();
    }

    void OnClickGuideConfrimButton()
    {
        guidePopup.HideUI();
        popup.HideUI();
        loadGamePanel.GetComponent<PopupUI>().ShowUI();
        loadGamePanel.GetComponent<LoadGameUI>().OnClickEditButton();
    }

    void OnClickGuideCancelButton()
    {
        guidePopup.HideUI();
        popup.HideUI();
    }
}
