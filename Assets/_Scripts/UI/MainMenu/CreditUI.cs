using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditUI : MonoBehaviour
{
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
        // TODO : 선택한 슬롯에 저장된 profileId로 게임 시작하기
    }

    void OnClickCancelButton()
    {
        popup.HideUI();
    }
}
