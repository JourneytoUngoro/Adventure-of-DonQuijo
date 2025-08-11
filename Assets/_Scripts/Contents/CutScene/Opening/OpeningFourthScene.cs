using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningFourthScene : MonoBehaviour, ICutScene
{
    [InfoBox("다른 컷으로 넘어가지 않기")]
    public bool isEditing;

    [Header("Fade")]
    public GameObject FadeObj;
    public float fadeDuratoin;

    [TabGroup("Don Quijote"), LabelText("Don Quijote Objects")]
    public GameObject DonquijoteObj;
    [TabGroup("Don Quijote")] [Tooltip("돈기호 탈출 전 시작 위치")]
    public Vector3 donStartPosition;
    [TabGroup("Don Quijote")] [Tooltip("돈기호 탈출 전 시작 스케일")]
    public Vector3 donStartScale;
    [TabGroup("Don Quijote")] [Tooltip("돈기호 탈출 전 시작 회전")]
    public Vector3 donStartRotate;
    [TabGroup("Don Quijote")] [Tooltip("돈기호 기본(기준) 위치")]
    public Vector3 donMiddlePosition;
    [TabGroup("Don Quijote")] [Tooltip("돈기호 기본(기준) 스케일")]
    public Vector3 donMiddleScale;
    [TabGroup("Don Quijote")] [Tooltip("돈기호 기본(기준) 회전")]
    public Vector3 donMiddleRotate;
    [TabGroup("Don Quijote")] [Tooltip("돈기호 마무리 이동 시 위치")]
    public Vector3 donEndPosition;
    [TabGroup("Don Quijote")] [Tooltip("돈기호 마무리 이동 시 스케일")]
    public Vector3 donEndScale;
    [TabGroup("Don Quijote")] [Tooltip("돈기호 마무리 이동 시 회전")]
    public Vector3 donEndRotate;
    [TabGroup("Don Quijote")] [Tooltip("돈기호 탈출 애니메이션 진행 시간")]
    public float donMidDuration;
    [TabGroup("Don Quijote")] [Tooltip("돈기호 마무리 이동 진행 시간")]
    public float donEndDuration;
    [TabGroup("Don Quijote")] [Tooltip("돈기호 탈출 애니메이션 두트윈 효과")]
    public Ease donMidEase; 
    [TabGroup("Don Quijote")] [Tooltip("돈기호 마무리 이동 애니메이션 두트윈 효과")]
    public Ease donEndEase;

    [TabGroup("Glass"), LabelText("Glass Objects")]
    public GameObject GlassObj;
    [TabGroup("Glass")] [Tooltip("유리 조각 탈출 전 시작 위치")]
    public Vector3 glassStartPosition;
    [TabGroup("Glass")] [Tooltip("유리 조각 탈출 전 시작 스케일")]
    public Vector3 glassStartScale;
    [TabGroup("Glass")] [Tooltip("유리 조각 탈출 전 시작 회전")]
    public Vector3 glassStartRotate;
    [TabGroup("Glass")] [Tooltip("유리 조각 기본(기준) 위치")]
    public Vector3 glassMiddlePosition;
    [TabGroup("Glass")] [Tooltip("유리 조각 기본(기준) 스케일")]
    public Vector3 glassMiddleScale;
    [TabGroup("Glass")] [Tooltip("유리 조각 기본(기준) 회전")]
    public Vector3 glassMiddleRotate;
    [TabGroup("Glass")] [Tooltip("유리 조각 마무리 이동 시 위치")]
    public Vector3 glassEndPosition;
    [TabGroup("Glass")] [Tooltip("유리 조각 마무리 이동 시 스케일")]
    public Vector3 glassEndScale;
    [TabGroup("Glass")] [Tooltip("유리 조각 마무리 이동 시 회전")]
    public Vector3 glassEndRotate;
    [TabGroup("Glass")] [Tooltip("유리 조각 탈출 애니메이션 진행 시간")]
    public float glassMidDuration;
    [TabGroup("Glass")] [Tooltip("유리 조각 마무리 이동 진행 시간")]
    public float glassEndDuration;
    [TabGroup("Glass")] [Tooltip("유리 조각 탈출 애니메이션 두트윈 효과")]
    public Ease glassMidEase; 
    [TabGroup("Glass")] [Tooltip("유리 조각 마무리 이동 애니메이션 두트윈 효과")]
    public Ease glassEndEase;

    public GameObject endImage;
    public Action onFinish { get ; set; }

    public void InitializeScene()
    {
        DonquijoteObj.transform.position = donStartPosition;
        DonquijoteObj.transform.localRotation = Quaternion.Euler(donStartRotate);
        DonquijoteObj.transform.localScale = donStartScale;

        GlassObj.transform.position = glassStartPosition;
        GlassObj.transform.localRotation = Quaternion.Euler(glassStartRotate);
        GlassObj.transform.localScale = glassStartScale;

        if (/*!FadeObj.activeSelf*/ true)
        {
            FadeObj.SetActive(true);
            SpriteRenderer fadeSp = FadeObj.GetComponent<SpriteRenderer>();
            Color newColor = fadeSp.color;
            newColor.a = 0;
            fadeSp.color = newColor;
        }
    }

    public void PlayScene()
    {
        Sequence donSequence = DOTween.Sequence();

        donSequence.Append(
            DonquijoteObj.transform.DOMove(donMiddlePosition, donMidDuration).SetEase(donMidEase)
            );
        donSequence.Join(
            DonquijoteObj.transform.DOScale(donMiddleScale, donMidDuration).SetEase(donMidEase)
            );
        donSequence.Join(
            DonquijoteObj.transform.DORotate(donMiddleScale, donMidDuration).SetEase(donMidEase)
            );

        donSequence.Append(
            DonquijoteObj.transform.DOMove(donEndPosition, donEndDuration).SetEase(donEndEase)
            );
        donSequence.Join(
            DonquijoteObj.transform.DOScale(donEndScale, donEndDuration).SetEase(donEndEase)
            );
        donSequence.Join(
            DonquijoteObj.transform.DORotate(donEndScale, donEndDuration).SetEase(donEndEase)
            );

        Sequence glassSequence = DOTween.Sequence();

        glassSequence.Append(
            GlassObj.transform.DOMove(glassMiddlePosition, glassMidDuration).SetEase(glassMidEase)
            );
        glassSequence.Join(
            GlassObj.transform.DOScale(glassMiddleScale, glassMidDuration).SetEase(glassMidEase)
            );
        glassSequence.Join(
            GlassObj.transform.DORotate(glassMiddleRotate, glassMidDuration).SetEase(glassMidEase)
            );
        glassSequence.Append(
            GlassObj.transform.DOMove(glassEndPosition, glassEndDuration).SetEase(glassEndEase)
        );
        glassSequence.Join(
            GlassObj.transform.DOScale(glassEndScale, glassEndDuration).SetEase(glassEndEase)
        );
        glassSequence.Join(
            GlassObj.transform.DORotate(glassEndRotate, glassEndDuration).SetEase(glassEndEase)
        );

        Sequence master = DOTween.Sequence();
        master.Append(donSequence);
        master.Join(glassSequence).OnComplete(() => { endImage.SetActive(true); if (!isEditing) onFinish?.Invoke(); });
    }

}
