using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///  UIManager.Instance.Get(UIType.type)...;
/// </summary>
public class TextInfoUI : UIBase
{
    // 오브젝트                      // 씬 상의 오브젝트 이름
    private TextMeshProUGUI infoTMP; // Info TMP
    private Image backgroundImage; // Background Image

    public bool canOverlap = false;

    protected override void AllowmentComponent()
    {
        infoTMP = GetComponentInChildren<TextMeshProUGUI>();
        backgroundImage = GetComponent<Image>();

        canOverlap = false;
    }

    public override void ShowUI(TweenCallback onFadeInComplete = null)
    {
        // Show the new text and hide the previous one (only one should be visible at a time)
        if (!canOverlap)
        {
            Manager.Instance.uiManager.HideCurrentTextInfoUI();
        }

        Manager.Instance.uiManager.OpenTextInfoUI(this);
        
        base.ShowUI(onFadeInComplete);
    }

    public override void HideUI(TweenCallback onFadeOutComplete = null)
    {
        // if this text is current one, UIManager should know it will be hidden
        if (Manager.Instance.uiManager.CheckCurrentTextInfo(this))
        {
            Manager.Instance.uiManager.HideCurrentTextInfoUI();
        }
        base.HideUI(onFadeOutComplete);
    }

    public override void ShowAndHideUI(float waitTime, TweenCallback onFadeInComplete = null, TweenCallback onFadeOutComplete = null)
    {
        // Only one text ui should be displayed
        if (!canOverlap)
        {
            Manager.Instance.uiManager.HideCurrentTextInfoUI();
        }
        Manager.Instance.uiManager.OpenTextInfoUI(this);

        base.ShowAndHideUI(waitTime, onFadeInComplete, onFadeOutComplete);
    }

    /// <summary>
    /// 동적으로 텍스트 창을 생성한다.
    /// </summary>
    public TextInfoUI SetDynamicTextInfo(TextInfoData data)
    {
        if (data.info != null && infoTMP) infoTMP.text = data.info;
        if (data.background != null && backgroundImage != null) backgroundImage.sprite = data.background;

        return this;
    }

    public void SetAnchoredPosition(int x, int y)
    {
        rectTransform.anchoredPosition = new Vector2(x, y);

    }

    protected override void ReturnToPool()
    {
        if (type == UIType.DynamicTextInfo) 
        Manager.Instance.uiManager.textInfoPool.Return(this);
    }

}

public class TextInfoData
{
    public string info;
    public Sprite background;

    public TextInfoData(string info,  Sprite background = null)
    {
        this.info = info;
        this.background = background;
    }

    public static TextInfoData FromTemplate(TextInfoData data)
    {
        return new TextInfoData(data.info, data.background);
    }

}