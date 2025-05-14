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
            TextInfoUI text =  Manager.Instance.uiManager.ShowDynamicTextInfo(new TextInfoData("새 슬롯을 선택해주세요")).GetComponent<TextInfoUI>();
            text.SetAnchoredPositioin(0, -200);
            text.ShowAndHideUI(2.5f);
        }
        else
        {
            TextInfoUI text = Manager.Instance.uiManager.ShowDynamicTextInfo(new TextInfoData("슬롯이 가득 찼습니다.")).GetComponent<TextInfoUI>();
            text.SetAnchoredPositioin(0, -200);
            text.ShowAndHideUI(2.5f);
            loadGamePanel.GetComponent<LoadGameUI>().OnClickEditButton();
        }

        
    }

    void OnClickCancelButton()
    {
        popup.HideUI();
    }
}
