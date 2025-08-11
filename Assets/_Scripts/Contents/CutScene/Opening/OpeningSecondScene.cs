using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningSecondScene : MonoBehaviour, ICutScene
{
    [InfoBox("다른 컷으로 넘어가지 않기")]
    public bool isEditing;

    public float waitTimeToStart;

    [TabGroup("Knight"), LabelText("Knight Obj")]
    public GameObject KnightObj;
    [TabGroup("Knight")] [Tooltip("기사 시작 위치")]
    public Vector3 knightStartPosition;
    [TabGroup("Knight")] [Tooltip("기사 시작 스케일")]
    public Vector3 knightStartScale;
    [TabGroup("Knight")] [Tooltip("기사 최종 위치")]
    public Vector3 knightEndPosition;
    [TabGroup("Knight")] [Tooltip("기사 최종 스케일")]
    public Vector3 knightEndScale;
    [TabGroup("Knight")] [Tooltip("기사 애니메이션 효과 진행 시간")]
    public float knightDuration;
    [TabGroup("Knight")] [Tooltip("기사 애니메이션 두트윈 효과")]
    public Ease knightEase;


    [TabGroup("Background"), LabelText("Background Obj")]
    public GameObject BackgroundObj;
    [TabGroup("Background")] [Tooltip("배경 시작 위치")]
    public Vector3 bgStartPosition;
    [TabGroup("Background")] [Tooltip("배경 시작 스케일")]
    public Vector3 bgStartScale;
    [TabGroup("Background")] [Tooltip("배경 최종 위치")]
    public Vector3 bgEndPosition;
    [TabGroup("Background")] [Tooltip("배경 최종 스케일")]
    public Vector3 bgEndScale;
    [TabGroup("Background")] [Tooltip("배경 애니메이션 효과 진행 시간")]
    public float bgDuration;
    [TabGroup("Background")] [Tooltip("배경 애니메이션 두트윈 효과")]
    public Ease bgEase;

    [TabGroup("Light"), LabelText("Light Obj")]
    public GameObject LightObj;
    [TabGroup("Light")] [Tooltip("빛 시작 스케일")]
    public Vector3 lightStartScale;
    [TabGroup("Light")] [Tooltip("빛 확대 시 최대 스케일")]
    public Vector3 lightMaxScale;
    [TabGroup("Light")] [Tooltip("빛 기본 스케일")]
    public Vector3 lightNormalScale;
    [TabGroup("Light")] [Tooltip("빛 최대 확대 시 회전")]
    public Vector3 lightMaxRotate;
    [TabGroup("Light")] [Tooltip("빛 기본 회전")]
    public Vector3 lightNormalRotate;
    [TabGroup("Light")] [Tooltip("빛 스케일 진행 시간")]
    public float lightScaleDuration;
    [TabGroup("Light")]
    public Ease lightEase;

    public Action onFinish { get; set; }

    private SpriteRenderer knightSp;
    private SpriteRenderer lightSp;
    private SpriteRenderer bgSp;

    public void InitializeScene()
    {
        // InitializeObjColor();

        KnightObj.transform.localPosition = knightStartPosition;
        KnightObj.transform.localScale = knightStartScale;
        LightObj.transform.localScale = lightStartScale;
        LightObj.transform.rotation = Quaternion.identity;
        BackgroundObj.transform.localPosition = bgStartPosition;
        BackgroundObj.transform.localScale = bgStartScale;
    }

    public void PlayScene()
    {
        Sequence master = DOTween.Sequence();

        // knight
        Sequence knightSequence = DOTween.Sequence();
        knightSequence.Append(
            KnightObj.transform.DOMoveY(knightEndPosition.y, knightDuration).SetEase(knightEase)
            );
        knightSequence.Join(
            KnightObj.transform.DOScale(knightEndScale, knightDuration).SetEase(knightEase)
            );

        // background
        Sequence bgSequence = DOTween.Sequence();
        bgSequence.Append(
            BackgroundObj.transform.DOMoveY(bgEndPosition.y, bgDuration).SetEase(bgEase)
            );
        bgSequence.Join(
            BackgroundObj.transform.DOScale(bgEndScale, bgDuration).SetEase(bgEase)
            );

        // light
        Sequence lightSequence = DOTween.Sequence();
        lightSequence.Append(
            LightObj.transform.DOScale(lightMaxScale, lightScaleDuration).SetEase(lightEase)
            );
        lightSequence.Join(
            LightObj.transform.DORotate(lightMaxRotate, lightScaleDuration).SetEase(Ease.Linear)
            );
        lightSequence.Append(
            LightObj.transform.DOScale(lightNormalScale, lightScaleDuration).SetEase(lightEase)
            );
        lightSequence.Join(
            LightObj.transform.DORotate(lightNormalRotate, lightScaleDuration).SetEase(Ease.Linear)
            );

        // master.Append(transitionSequence);
        master.AppendInterval(waitTimeToStart);
        master.Append(knightSequence);
        master.Join(bgSequence);
        master.Append(lightSequence);
        master.AppendInterval(1.5f).OnComplete(() => { if (!isEditing) onFinish?.Invoke(); });

    }
}
