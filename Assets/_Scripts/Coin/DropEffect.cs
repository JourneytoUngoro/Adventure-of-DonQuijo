using UnityEngine;
using DG.Tweening;
using System.Collections;

public class DropEffect : MonoBehaviour
{
    [Header("Motion Settings")]
    public float jumpHeight = 13f;
    public float jumpDuration = 0.15f;
    public float horizontalDistance = 15f;
    public float horizontalmultiplier = 1.5f; // falled position
    public float fallDuration = 0.4f;
    public float bounceDuration = 0.5f;

    [Header("Ease Settings")]
    public Ease jumpEffect = Ease.OutQuad;
    public Ease fallHorizontalEffect = Ease.Linear;
    public Ease bounceEffect = Ease.OutBounce;
/*
    [Header("Additional Effect")]
    public GameObject shadow;*/

    [HideInInspector] public SoundPlayer dropSoundPlayer;

    private CoinObject coinObject;

    private void Start()
    {
        coinObject = GetComponent<CoinObject>();
    }

    public void PlayDropEffect(Vector3 startPos, Vector3 endPos)
    {
        int randomDirX = Random.Range(0, 3) == 0 ? -1 : ( Random.Range(0, 2) == 1 ? 0 :1); // 33% - left, mid, right
        float randomHorizontalDistance = horizontalDistance * randomDirX;
        float randomFinalRangeX = Random.Range(30f, 70f) * randomDirX;
        if (randomDirX == 0)
        {
            randomHorizontalDistance = horizontalDistance * 0.3f * (Random.Range(0, 2) == 1 ? -1 : 1);
            randomFinalRangeX = Random.Range(30f, 70f) *0.3f * (Random.Range(0, 2) == 1 ? -1 : 1);
        }

        int randomDirY = Random.Range(0, 2) == 0 ? -1 : 1; // 50% - top, bottom
        float randomFinalRangeY = Random.Range(15f, 25f) * randomDirY;

        Vector3 peakPos = startPos + new Vector3(randomHorizontalDistance, jumpHeight, 0);
        Vector3 finalPos = new Vector3(
            startPos.x + randomHorizontalDistance * horizontalmultiplier + randomFinalRangeX, 
            endPos.y + randomFinalRangeY,
            endPos.z
            );

        transform.position = startPos;

        Sequence dropSequence = DOTween.Sequence();
        dropSequence.Append(transform.DOMove(peakPos, jumpDuration).SetEase(jumpEffect));
        dropSequence.Append(transform.DOMoveY(finalPos.y, fallDuration).SetEase(bounceEffect));
        dropSequence.Join(transform.DOMoveX(finalPos.x, bounceDuration).SetEase(fallHorizontalEffect));

        dropSoundPlayer = null;
        Manager.Instance.soundManager.PlaySoundFXClip(out dropSoundPlayer, "coinDropSFX", transform);

    }
}
