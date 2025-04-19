using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum UIType
{
    StaticPopup,
    pausePopup, // 메인 팝업
    settingPopup, //  설정 팝업
    VendingMachine, 
    PurchasePopup, // 구매 팝업
    QuestPopup, // 퀘스트 진행 팝업
    TextInfo, // 텍스트 정보 
    DynamicPopup, // 동적으로 생성되는 팝업
    DynamicTextInfo, // 동적으로 생성되는 텍스트 정보
    DynamicImage,  // 동적으로 생성되는 이미지 
 }

[RequireComponent(typeof(CanvasGroup))]
public abstract class UIBase : MonoBehaviour
{
    public UIType type;

    public float fadeTime;
    public float delayTime;
    [field: SerializeField] public bool BlockClick { get; private set; }

    protected CanvasGroup group;
    protected RectTransform rectTransform;

    [Tooltip("the position to appear when dynamically created")]
    public Vector2 position;

    private Coroutine showCoroutine = null;
    private Coroutine showAndHideCoroutine = null;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        group.alpha = 0;
        group.blocksRaycasts = false;

        AllowmentComponent();
    }

    protected virtual void AllowmentComponent() { }

    public virtual void ShowUI()
    {
        if (showCoroutine != null) { Manager.Instance.uiManager.StopCoroutine(showCoroutine); }
        
        rectTransform.SetAsLastSibling();
        showCoroutine = Manager.Instance.uiManager.StartCoroutine(ShowUICoroutine(delayTime));
    }

    public IEnumerator ShowUICoroutine(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        group.blocksRaycasts |= true;
        group.DOFade(1, fadeTime).SetUpdate(true);
    }

    public virtual void ShowAndHideUI(float waitTime)
    {
        if (showAndHideCoroutine != null) { Manager.Instance.uiManager.StopCoroutine(showAndHideCoroutine); }

        rectTransform.SetAsLastSibling();
        showAndHideCoroutine = Manager.Instance.uiManager.StartCoroutine(ShowAndHideCoroutine(waitTime));   
    }

    public IEnumerator ShowAndHideCoroutine(float waitTime)
    {
        group.blocksRaycasts = true;
        group.DOFade(1, fadeTime);

        yield return new WaitForSeconds(waitTime);

        group.DOFade(0, fadeTime);

        ReturnToPool();
        group.blocksRaycasts = false;
    }

    public virtual void HideUI()
    {
        rectTransform.SetAsFirstSibling();

        group.DOFade(0, fadeTime).SetUpdate(true);

        ReturnToPool();
        group.blocksRaycasts = false;
    }

    public virtual void Move(Vector2 direction, bool ease)
    {
        var tween = rectTransform.DOAnchorPos(direction, fadeTime);
        if (ease) tween.SetEase(Ease.OutBack, 0.9f);
    }

    protected virtual void ReturnToPool() { }


}
