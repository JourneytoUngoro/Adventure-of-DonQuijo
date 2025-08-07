using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Linq;

public class ButtonScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float hoverScale = 1.07f;
    [SerializeField] private float clickScale = 0.9f;
    [SerializeField] private float duration = 0.2f;
    public bool printDebug = false;

    public bool canEffect;

    private Vector3 originalScale;
    private Tweener currentTween;

    private void Awake()
    {
        originalScale = transform.localScale;

        canEffect = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!canEffect) return;

        AnimateScale(originalScale * hoverScale);
        if (printDebug)
        {
            Debug.Log($"Hover : {originalScale} * {hoverScale} -> {transform.localScale}");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!canEffect) return;

        AnimateScale(originalScale);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!canEffect) return;

        AnimateScale(originalScale * clickScale);

        if (printDebug)
        {
            Debug.Log($"Clicked : {originalScale} -> {transform.localScale}");
        }


        Manager.Instance.soundManager.PlayUI("buttonSFX");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!canEffect) return;

        // 손 떼면 hover 상태로 돌아가야 함
        AnimateScale(originalScale * hoverScale);
    }

    private void AnimateScale(Vector3 targetScale)
    {
        if (currentTween != null && currentTween.IsActive())
            currentTween.Kill();

        currentTween = transform.DOScale(targetScale, duration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true); // Time.timeScale 이 0이어도 작동
    }



}