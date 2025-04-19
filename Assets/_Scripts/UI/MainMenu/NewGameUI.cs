using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameUI : MonoBehaviour
{
    [SerializeField] GameObject loadGamePanel;

    PopupUI popup;

    private void Awake()
    {
        popup = GetComponent<PopupUI>();
    }

    private void Start()
    {
        popup.SetDynamicPopupEvent(OnClickConfirmButton, OnClickCancelButton);
    }

    void OnClickConfirmButton()
    {
        popup.HideUI();
        loadGamePanel.GetComponent<PopupUI>().ShowUI();
        if (Manager.Instance.dataManager.AllProfilesCount() < 3)
        {
            Manager.Instance.uiManager.ShowDynamicTextInfo(new TextInfoData("Choose New Slot")).ShowAndHideUI(5f);
        }
        else
        {
            Manager.Instance.uiManager.ShowDynamicTextInfo(new TextInfoData("Slots Full. Delete Slot")).ShowAndHideUI(3f);
            loadGamePanel.GetComponent<LoadGameUI>().OnClickEditButton();
        }

        
    }

    void OnClickCancelButton()
    {
        popup.HideUI();
    }
}
