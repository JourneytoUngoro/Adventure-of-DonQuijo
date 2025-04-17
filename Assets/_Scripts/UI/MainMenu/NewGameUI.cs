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
        if (Manager.Instance.dataManager.AllProfilesCount() < 3)
        {
            loadGamePanel.GetComponent<PopupUI>().ShowUI();
        }
        else
        {
            Manager.Instance.uiManager.ShowDynamicTextInfo(new TextInfoData("Slots Full. Delete Slot")).ShowAndHideUI(3f);
        }
        popup.HideUI();
        
        //loadGamePopup.ShowUI();
    }

    void OnClickCancelButton()
    {
        popup.HideUI();
    }
}
