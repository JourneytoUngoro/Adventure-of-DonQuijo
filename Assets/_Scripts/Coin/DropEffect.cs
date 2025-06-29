using UnityEngine;
using DG.Tweening;

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

    [HideInInspector] public SoundPlayer dropSoundPlayer;

    private CoinObject coinObject;
    private int randomDir;

    private void Start()
    {
        coinObject = GetComponent<CoinObject>();
    }

    public void PlayDropEffect(Vector3 startPos, Vector3 endPos)
    {
        int randomDir = Random.Range(0, 2) == 0 ? -1 : 1;
        horizontalDistance *= randomDir;

        float randomFinalRangeX = Random.Range(45f, 70f) * randomDir;
        randomDir = Random.Range(0, 2) == 0 ? -1 : 1;

        float randomFinalRangeY = Random.Range(10f, 20f) * randomDir;

        Vector3 peakPos = startPos + new Vector3(horizontalDistance, jumpHeight, 0);
        Vector3 finalPos = new Vector3(
            startPos.x + horizontalDistance * horizontalmultiplier + randomFinalRangeX, 
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
