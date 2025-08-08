using DG.Tweening;
using UnityEngine;
using System.Linq;

public class VendingMachineSlotEffect : MonoBehaviour
{
    [SerializeField] private float selectedScale = 1.05f;
    [SerializeField] private float duration = 0.15f;

    private Vector3 originalScale;
    private Tweener currentTweener;

    public bool canEffect;


    private void Start()
    {
        originalScale = transform.localScale;
        canEffect = true;
    }

    public void OnSelectedState()
    {
        if (!canEffect) return;

        AnimateScale(originalScale * selectedScale);
        Manager.Instance.soundManager.PlayUI("vendingMachinSlotSFX");
    }

    public void OnUnSelectedState()
    {
        if (!canEffect) return;

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
