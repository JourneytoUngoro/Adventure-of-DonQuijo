using UnityEngine;
using DG.Tweening;

public class UIFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    public void FadeIn()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        canvasGroup.DOFade(1f, fadeDuration);
    }

    public void FadeOut()
    {
        canvasGroup.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        });
    }
}
