using Sirenix.OdinInspector;
using System;
using UnityEngine;
using DG.Tweening;

public class OpeningThirdScene : MonoBehaviour, ICutScene
{
    [InfoBox("다른 컷으로 넘어가지 않기")]
    public bool isEditing;

    [Header("Transition duration")]
    public float transitionTime; 

    [TabGroup("Hand"), LabelText("Hand Obj")]
    public GameObject HandObj;
    [TabGroup("Hand")] [Tooltip("손 시작 위치")]
    public Vector3 handStartPosition;
    [TabGroup("Hand")] [Tooltip("손 시작 스케일")]
    public Vector3 handStartScale;
/*    [TabGroup("Hand")] 
    public Vector3 handEndPosition;
    [TabGroup("Hand")]
    public Vector3 handEndScale;*/

    [TabGroup("Background")]
    public GameObject BackgroundObj;
/*    public GameObject TopCloseObj;
    public GameObject BottomCloseObj;
    public float closeTime;
*/
    private Animator handAnim;
    private SpriteRenderer handSp;
    private SpriteRenderer bgSp;

    public Action onFinish { get; set; }

    public void InitializeScene()
    {
        handAnim = GetComponentInChildren<Animator>();

        HandObj.transform.position = handStartPosition;
        HandObj.transform.localScale = handStartScale;
    }

    public  void PlayScene()
    {
        Sequence transitionSequence = DOTween.Sequence();

        transitionSequence.AppendInterval(0.2f).OnComplete(() => { handAnim.Play("HandMove"); });
    }

    public void OnFinishHandAnimation()
    {
        Sequence waitSequence = DOTween.Sequence();
        waitSequence.AppendInterval(1.5f).OnComplete(() => { if (!isEditing) onFinish?.Invoke(); });
    }
}
