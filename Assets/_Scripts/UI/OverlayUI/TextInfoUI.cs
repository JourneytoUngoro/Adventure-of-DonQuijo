using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///  UIManager.Instance.Get(UIType.type)...;
/// </summary>
public class TextInfoUI : UIBase
{
    // 오브젝트                      // 씬 상의 오브젝트 이름
    TextMeshProUGUI infoTMP; // Info TMP
    Image backgroundImage; // Background Image

    protected override void AllowmentComponent()
    {
        infoTMP = GetComponentInChildren<TextMeshProUGUI>();
        backgroundImage = GetComponent<Image>();

    }

    public override void ShowUI()
    {
        base.ShowUI();
    }

    public override void HideUI()
    {
        base.HideUI();
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
    /// 동적으로 텍스트 창을 생성한다.
    /// </summary>
    public TextInfoUI SetDynamicTextInfo(TextInfoData data)
    {
        if (data.info != null && infoTMP) infoTMP.text = data.info;
        if (data.background != null && backgroundImage != null) backgroundImage.sprite = data.background;

        rectTransform.anchoredPosition = position;

        return this;
    }

    public void SetAnchoredPositioin(int x, int y)
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