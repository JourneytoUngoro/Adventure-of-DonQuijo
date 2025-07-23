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
            TextInfoUI text =  Manager.Instance.uiManager.ShowDynamicTextInfo(new TextInfoData("\t새 슬롯을 선택해주세요\t")).GetComponent<TextInfoUI>();
            text.SetAnchoredPositioin(0, -400);
            text.ShowAndHideUI(2.5f);
        }
        else
        {
            TextInfoUI text = Manager.Instance.uiManager.ShowDynamicTextInfo(new TextInfoData("\t슬롯이 가득 찼습니다.\t")).GetComponent<TextInfoUI>();
            text.SetAnchoredPositioin(0, -400);
            text.ShowAndHideUI(2.5f);
            loadGamePanel.GetComponent<LoadGameUI>().OnClickEditButton();
        }

        
    }

    void OnClickCancelButton()
    {
        popup.HideUI();
    }
}
