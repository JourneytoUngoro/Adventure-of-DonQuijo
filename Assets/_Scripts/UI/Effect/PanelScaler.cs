using DG.Tweening;
using UnityEngine;
using System.Linq;

public class PanelScaler : MonoBehaviour
{
    [SerializeField] private float selectedScale = 1.05f;
    [SerializeField] private float duration = 0.15f;

    private Vector3 originalScale;
    private Tweener currentTweener;


    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnSelectedState()
    {
        AnimateScale(originalScale * selectedScale);
        Manager.Instance.soundManager.PlayUI("buttonSFX");
    }

    public void OnUnSelectedState()
    {
        AnimateScale(originalScale);
    }

    private void AnimateScale(Vector3 targetScale)
    {
        if (currentTweener != null && currentTweener.IsActive())
        {
            currentTweener.Kill();
        }

        currentTweener = transform.DOScale(targetScale, duration).SetEase(Ease.OutBack);
    }
}
