using DG.Tweening;
using System.Collections;
using System;
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
    FadeImage, // 페이드 효과 이미지
    DynamicPopup, // 동적으로 생성되는 팝업
    DynamicTextInfo, // 동적으로 생성되는 텍스트 정보
    DynamicImage,  // 동적으로 생성되는 이미지
    LoadingBar, // 로딩바
    PlayerStatus // 플레이어 상태(체력, 체간 등)
 }

[RequireComponent(typeof(CanvasGroup))]
public abstract class UIBase : MonoBehaviour
{
    public UIType type;

    [SerializeField, Min(0.0f)] private float fadeTime;
    [SerializeField, Min(0.0f)] private float delayTime;

    public bool isOpened { get; set; }

    [field: SerializeField] public bool BlockClick { get; private set; }

    protected CanvasGroup group;
    protected RectTransform rectTransform;

    [field: SerializeField, Tooltip("the position to appear when dynamically created")]
    public Vector2 position { get; protected set; }

    private Coroutine showCoroutine = null;
    private Coroutine showAndHideCoroutine = null;

    public Action onShow { get; private set; }
    public Action onHide { get; private set; }
   
    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        group.alpha = 0;
        group.blocksRaycasts = false;

        AllowmentComponent();
    }

    public virtual void ShowUI(TweenCallback onFadeInComplete = null)
    {
        onShow?.Invoke();

        if (showCoroutine != null)
        {
            // Manager.Instance.uiManager.StopCoroutine(showCoroutine);
            StopCoroutine(showCoroutine);
        }
        
        rectTransform.SetAsLastSibling();

        // showCoroutine = Manager.Instance.uiManager.StartCoroutine(ShowUICoroutine(delayTime));
        showCoroutine = StartCoroutine(ShowUICoroutine(delayTime, onFadeInComplete));

        isOpened = true;
    }

    private IEnumerator ShowUICoroutine(float delayTime, TweenCallback onFadeInComplete)
    {
        yield return new WaitForSeconds(delayTime);

        group.blocksRaycasts = true;
        group.DOFade(1, fadeTime).SetUpdate(true).OnComplete(onFadeInComplete);
    }

    public virtual void HideUI(TweenCallback onFadeOutComplete = null)
    {
        // Debug.Assert(rectTransform != null, "rectTransform null!");
        rectTransform.SetAsFirstSibling();

        group.DOFade(0, fadeTime).SetUpdate(true).OnComplete(onFadeOutComplete);

        ReturnToPool();
        group.blocksRaycasts = false;
        isOpened = false;

        onHide?.Invoke();
    }

    public virtual void ShowAndHideUI(float waitTime, TweenCallback onFadeInComplete = null, TweenCallback onFadeOutComplete = null)
    {
        if (showAndHideCoroutine != null)
        {
            StopCoroutine(showAndHideCoroutine);
            // Manager.Instance.uiManager.StopCoroutine(showAndHideCoroutine);
        }

        rectTransform.SetAsLastSibling();
        // showAndHideCoroutine = Manager.Instance.uiManager.StartCoroutine(ShowAndHideCoroutine(waitTime));
        showAndHideCoroutine = StartCoroutine(ShowAndHideCoroutine(waitTime, onFadeInComplete, onFadeOutComplete));
    }

    private IEnumerator ShowAndHideCoroutine(float waitTime, TweenCallback onFadeInComplete, TweenCallback onFadeOutComplete)
    {
        isOpened = true;
        group.blocksRaycasts = true;
        group.DOFade(1, fadeTime).SetUpdate(true).OnComplete(onFadeInComplete);

        yield return new WaitForSeconds(waitTime);

        group.DOFade(0, fadeTime).SetUpdate(true).OnComplete(onFadeOutComplete);

        ReturnToPool();
        group.blocksRaycasts = false;
        isOpened = false;
    }

    public virtual void ShowAndHideUI(Func<bool> waitUntilCondition, TweenCallback onFadeInComplete = null, TweenCallback onFadeOutComplete = null)
    {
        if (showAndHideCoroutine != null)
        {
            StopCoroutine(showAndHideCoroutine);
        }

        rectTransform.SetAsLastSibling();
        showAndHideCoroutine = StartCoroutine(ShowAndHideCoroutine(waitUntilCondition, onFadeInComplete, onFadeOutComplete));
    }

    private IEnumerator ShowAndHideCoroutine(Func<bool> waitUntilCondition, TweenCallback onFadeInComplete, TweenCallback onFadeOutComplete)
    {
        isOpened = true;
        group.blocksRaycasts = true;
        group.DOFade(1, fadeTime).SetUpdate(true).OnComplete(onFadeInComplete);

        yield return new WaitUntil(waitUntilCondition);

        group.DOFade(0, fadeTime).SetUpdate(true).OnComplete(onFadeOutComplete);

        ReturnToPool();
        group.blocksRaycasts = false;
        isOpened = false;
    }

    // TODO: 움직이는 기능 구현
    public virtual void Move(RectTransform direction, bool ease)
    {
        var tween = rectTransform.DOAnchorPos(direction.anchoredPosition, fadeTime);
        if (ease) tween.SetEase(Ease.OutBack, 0.9f);
    }

    protected abstract void ReturnToPool();
    protected abstract void AllowmentComponent();

    public RectTransform GetRectTransform() => rectTransform;

    public void SetOnShow(Action onShow) => this.onShow = onShow;
    public void SetOnHide(Action onHide) => this.onHide = onHide;

}
