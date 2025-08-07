using System;
using System.Collections;
using UnityEngine;

public class MemoryFragmentObject : InteractBase
{
    public LayerMask playerMask;

    public QuestInfoSO relatedQuest;
    private string questId;

    public float mfLifeTime = 4f;
    public Timer timer {  get; private set; }

    public float moveSpeed = 1f;
    public float epsilon = 2.0f;

    private Player player;
    private bool isChasingPlayer;

    public PooledObject pooledObject {  get; private set; }
    public DropEffect dropEffect { get; private set; }

    public Action<MemoryFragmentObject> onRelease;

    private Coroutine chasingPlayerCoroutine;

    public void Start()
    {
        // TODO : Player 의존성 주입 필요
        player = FindAnyObjectByType<Player>();

        pooledObject = GetComponent<PooledObject>();
        dropEffect = GetComponent<DropEffect>();
        dropEffect.isCoin = false;

        isChasingPlayer = false;
        questId = relatedQuest.id;

        chasingPlayerCoroutine = null;
    }

    public void SetTimer()
    {
        timer = new Timer(mfLifeTime);
    }
    public void SetTimer(float time)
    {
        timer = new Timer(time);
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (isChasingPlayer) { return; }

        if (chasingPlayerCoroutine != null) { chasingPlayerCoroutine = null; }
        chasingPlayerCoroutine = StartCoroutine(NearToPlayer());
    }
    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        if (isChasingPlayer || chasingPlayerCoroutine != null)
        {
            isChasingPlayer = false;
            StopCoroutine(chasingPlayerCoroutine);
            chasingPlayerCoroutine = null;
        }

    }

    private IEnumerator NearToPlayer()
    {
        isChasingPlayer = true;

        Vector3 dir = player.transform.position - transform.position;
        while (dir.magnitude > epsilon)
        {
            transform.position += dir.normalized * moveSpeed;
            dir = player.transform.position - transform.position;

            yield return null;
        }

        HandlePlayerContact();
    }

    private void HandlePlayerContact()
    {
        isChasingPlayer = false;

        if (dropEffect.dropSoundPlayer != null && dropEffect.dropSoundPlayer.isPlaying)
        {
            dropEffect.dropSoundPlayer.StopPlayingAudioClip();
        }

        // TODO : change audioClip to  "memoryCollectSFX"
        Manager.Instance.soundManager.PlaySoundFXClip("mfCollectSFX", transform);

        OnObtainMemoryFragment();
 
        // Always return this object to the pool when done.
        pooledObject.ReleaseObject();
        onRelease?.Invoke(this);
    }

    public void OnObtainMemoryFragment()
    {
        Manager.Instance.questManager.GetQuestById(questId).UpdateProgress();
        Manager.Instance.stageManager.nowStage.UpdateCollectedMemoryFragment();
    }

    public override void Interact()
    {
        throw new System.NotImplementedException();
    }
}
