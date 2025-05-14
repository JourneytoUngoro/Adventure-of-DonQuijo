using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class MainMenuEffect : MonoBehaviour
{
    [SerializeField] private RectTransform movingImage;

    [SerializeField] private Button[] buttonsToActivate;

    [SerializeField] private Vector2 targetAnchoredPosition;

    [SerializeField] private float moveDuration = 2f;
    [SerializeField] private float delayTime = 0.2f;

    private void Awake()
    {
        InitUI();
    }

    private void Start()
    {
        StartMenuEffect();
    }

    private void InitUI()
    {
        foreach (var button in buttonsToActivate)
        {
            button.gameObject.SetActive(false);
        }

        movingImage.anchoredPosition = Vector2.zero;
    }

    private void StartMenuEffect()
    {
        movingImage.DOAnchorPos(targetAnchoredPosition, moveDuration)
            .SetEase(Ease.OutCubic);

        DOVirtual.DelayedCall(moveDuration * 2f, () =>
        {
            StartCoroutine(ActivateButtonsSequentially());
        });
    }

    private IEnumerator ActivateButtonsSequentially()
    {
        foreach (var button in buttonsToActivate)
        {
            button.gameObject.SetActive(true);
            yield return new WaitForSeconds(delayTime);
        }
    }
}
