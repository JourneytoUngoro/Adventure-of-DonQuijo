using Cinemachine;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningFirstScene : MonoBehaviour, ICutScene
{
    [InfoBox("다른 컷으로 넘어가지 않기")]
    public bool isEditing;

    [Header("Fade")]
    public float fadeDuratoin; 

    [TabGroup("Don Quijote")]
    public GameObject DonquijoteObj;
    [TabGroup("Don Quijote")] [Tooltip("패럴랙스 시 오브젝트 시작 위치")]
    public Vector3 donStartPosition;
    [TabGroup("Don Quijote")] [Tooltip("패럴랙스 시 오브젝트 최종 위치")]
    public Vector3 donEndPosition;
    [TabGroup("Don Quijote")] [Tooltip("카메라 줌인 시 오브젝트 최종 위치")]
    public Vector3 donZoomPosition;
    [TabGroup("Don Quijote")] [Tooltip("패럴랙스 진행 시간")]
    public float donDuration;
    [TabGroup("Don Quijote")] [Tooltip("패럴랙스 시 오브젝트 이동 두트윈 효과")]
    public Ease donEase;


    [TabGroup("Background")]
    public GameObject BackgroundObj;
    [TabGroup("Background")] [Tooltip("패럴랙스 시 오브젝트 시작 위치")]
    public Vector3 bgStartPosition;
    [TabGroup("Background")] [Tooltip("패럴랙스 시 오브젝트 최종 위치")]
    public Vector3 bgEndPosition;
    [TabGroup("Background")] [Tooltip("패럴랙스 시 진행 시간")]
    public float bgDuration;
    [TabGroup("Background")] [Tooltip("패럴랙스 시 오브젝트 이동 두트윈 효과")]
    public Ease bgEase;


    [TabGroup("Camera")]
    public CinemachineVirtualCamera vCam1;
    [TabGroup("Camera")] [Tooltip("줌인 시 시작 위치")]
    public Vector3 vCam1StartPosition;
    [TabGroup("Camera")] [Tooltip("줌인 시 타겟 위치")]
    public Vector3 zoomEndPosition;
    [TabGroup("Camera")][Tooltip("줌인 시 시작 줌 배율")]
    public float vCam1StratOrthoSize; 
    [TabGroup("Camera")] [Tooltip("줌인 시 최종 줌 배율")]
    public float zoomOrthographicSize;
    [TabGroup("Camera")] [Tooltip("줌인 시 진행 시간")]
    public float zoomDuration;
    [TabGroup("Camera")] [Tooltip("줌인 시 시작 전 대기 시간")]
    public float beforeZoomWaitTime;
    [TabGroup("Camera")] [Tooltip("줌인 시 카메라 두트윈 효과")]
    public Ease zoomEase;

    public GameObject FadeObj;

    public Action onFinish { get; set; }
    private Color fadeColor;

    public void InitializeScene()
    {
        SetFadeEffect();

        DonquijoteObj.transform.position = donStartPosition;
        BackgroundObj.transform.position = bgStartPosition;
        vCam1.transform.position = vCam1StartPosition;
        vCam1.m_Lens.OrthographicSize = vCam1StratOrthoSize;
        vCam1.gameObject.SetActive(true);
    }

    public void PlayScene()
    {
        // fade in 
        Sequence fadeSequence = DOTween.Sequence();
        fadeSequence.Append(
            DOTween.To(() => FadeObj.GetComponent<SpriteRenderer>().color.a,
                                    x =>
                                    {
                                        Color color = FadeObj.GetComponent<SpriteRenderer>().color;
                                        color.a = x;
                                        FadeObj.GetComponent<SpriteRenderer>().color = color;
                                    },
                                    0,
                                    fadeDuratoin
            ));

        Sequence donBackgroundSequence = DOTween.Sequence();

        // don quijote's move
        donBackgroundSequence.Append(
            DonquijoteObj.transform.DOMove(donEndPosition, donDuration).SetEase(donEase)
            );
        // bg's move
        donBackgroundSequence.Join(
            BackgroundObj.transform.DOMove(bgEndPosition, bgDuration).SetEase(bgEase)
            );

        // vCam1 zoom and move
        Sequence zoomSequence = DOTween.Sequence();

        zoomSequence.Append(
            DOTween.To(() => vCam1.m_Lens.OrthographicSize,
                                    x => vCam1.m_Lens.OrthographicSize = x,
                                    zoomOrthographicSize,
                                    zoomDuration
            ).SetEase(zoomEase));

        zoomSequence.Join(
            vCam1.transform.DOMove(zoomEndPosition, zoomDuration).SetEase(zoomEase)
            );

        zoomSequence.Join(
            DonquijoteObj.transform.DOMove(donZoomPosition, zoomDuration).SetEase(zoomEase)
            );

        Sequence masterSequence = DOTween.Sequence();
        masterSequence.AppendInterval(fadeDuratoin / 2f).OnComplete(() => masterSequence.Join(donBackgroundSequence));
        masterSequence.Join(fadeSequence);
        masterSequence.AppendInterval(beforeZoomWaitTime);
        masterSequence.Append(zoomSequence);
        masterSequence.AppendInterval(0.2f).OnComplete(() => {
            if (!isEditing) { onFinish?.Invoke(); }
            vCam1.m_Lens.OrthographicSize = vCam1StratOrthoSize;
            vCam1.transform.position = vCam1StartPosition;
        });
    }

    private void SetFadeEffect()
    {
        FadeObj.SetActive(true);
        SpriteRenderer sp = FadeObj.GetComponent<SpriteRenderer>();
        fadeColor =sp.color;
        Color initialColor = fadeColor;
        initialColor.a = 1;
        sp.color = initialColor;
    }

}
