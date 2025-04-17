using JetBrains.Annotations;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
///  UIManager.Instance.Get(UIType.type)...;
/// </summary>

public class PopupUI : UIBase
{
    // 오브젝트
    public TextMeshProUGUI titleTMP; // Title TMP
    public TextMeshProUGUI infoTMP; // Info TMP
    public Button confirmButton; // Confirm Button
    public Button cancelButton; // Cancel Button

    // Starting Popup으로 씬 상에 미리 생성되어 있는 팝업창

    protected override void AllowmentComponent()
    {
        titleTMP = transform.Find("Title TMP")?.GetComponent<TextMeshProUGUI>();
        infoTMP = transform.Find("Info TMP")?.GetComponent <TextMeshProUGUI>();
        confirmButton = transform.Find("Confirm Button")?.GetComponent<Button>();
        cancelButton = transform.Find ("Cancel Button")?.GetComponent<Button>();
    }

    public override void ShowUI()
    {
        base.ShowUI();
        Manager.Instance.uiManager.activatedPopups.Push(this);
    }

    public override void HideUI()
    {
        base.HideUI();

        Manager.Instance.uiManager.activatedPopups.TryPop(out _);
    }

    public override void ShowAndHideUI(float waitTime)
    {
        base.ShowAndHideUI(waitTime);
    }

    public override void Move(Vector2 direction, bool ease)
    {
        // TODO : 패널 움직이는 기능 구현 
        base.Move(direction, ease);
    }

    /// <summary>
    /// PopupData를 이용하여 동적으로 팝업을 생성한다. 제목, 내용, 확인 및 취소 콜백을 설정할 수 있다.
    /// SetDynamicPopup(data).ShowDynamicPopup();
    /// </summary>
    public PopupUI SetDynamicPopup(PopupData data)
    {
        // Dynamic PopupUI
        if (data.title != null && titleTMP) titleTMP.text = data.title;
        if (data.info != null && infoTMP) infoTMP.text = data.info;

        confirmButton?.onClick.RemoveAllListeners();
        cancelButton?.onClick.RemoveAllListeners();

        // 확인 버튼
        if (confirmButton != null)
        {
            confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = data.confirmText;
        }
        else
        {
            Debug.Log("cannot find confirm button");
        }

        // 취소 버튼 
        if (cancelButton != null)
        {
            cancelButton.GetComponentInChildren<TextMeshProUGUI>().text = data.cancelText;

            if (data.cancelText == null)
            {
                // Cancel 이벤트가 없는 경우, 즉 Cancel 버튼이 없는 경우
                cancelButton.gameObject.SetActive(false);

                // 확인 버튼을 가운데로 이동
                // 혹은 Horizontal Layout Group을 이용하여 아래 과정을 생략한다 
                var rect = confirmButton.GetComponent<RectTransform>();
                var pos = rect.anchoredPosition;
                pos.x = 0f;
                rect.anchoredPosition = pos;
            }
        }
        else
        {
            Debug.Log("cannot find cancel  button");
        }

        rectTransform.anchoredPosition = position;

        return this;
    }

    public void SetDynamicPopupEvent(UnityAction onConfirm, UnityAction onCancel)
    {
        // 확인 버튼
        if (confirmButton != null && onConfirm != null)
        {
           confirmButton?.onClick.AddListener(onConfirm);
        }

        // 취소 버튼 
        if (cancelButton != null && onCancel != null)
        {

           cancelButton?.onClick.AddListener(onCancel);
        }
    }

    protected override void ReturnToPool()
    {
        if (type == UIType.DynamicPopup)
            Manager.Instance.uiManager.popupPool.Return(this);
    }

}

// Starting Popup이 아닌 동적으로 생성할 때 필요한 데이터 클래스
public class PopupData
{
    public string title;
    public string info;
    public string confirmText;
    public string cancelText;

    public PopupData(string title, string info, string confirmText, string cancelText)
    {
        this.title = title; this.info = info; this.confirmText = confirmText; this.cancelText = cancelText;
    }

    // Scriptable Object를 이용해서 팝업을 생성
    public static PopupData FromTemplate(PopupTemplateSO template)
    {
        return new PopupData
        (
            template.title, template.info, template.confirmText, template.cancelText
        );
    }
}