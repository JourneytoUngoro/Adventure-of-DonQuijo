using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryFragmentObject : InteractBase
{
    public LayerMask playerMask;

    public float moveSpeed = 1f;
    public float epsilon = 2.0f;

    private Player player;
    private bool isChasingPlayer;

    private PooledObject pooledObject;
    private DropEffect dropEffect;

    public void Start()
    {
        // TODO : Player 의존성 주입 필요
        player = FindAnyObjectByType<Player>();

        pooledObject = GetComponent<PooledObject>();
        dropEffect = GetComponent<DropEffect>();

        isChasingPlayer = false;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        Debug.Log("충돌했다@"+ collision.gameObject.name +  "와 " + collision.gameObject.transform.position + "에서");

        if (isChasingPlayer) { return; }

        StartCoroutine(NearToPlayer());
    }

    private IEnumerator NearToPlayer()
    {
        Debug.Log("왜 이동함?");
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
        Manager.Instance.soundManager.PlaySoundFXClip("coinCollectSFX", transform);
        
        // TODO : add quest handle logic 
        // Manager.Instance.questManager.UpdateQuestState(Quest quest);

        // Always return this object to the pool when done.
        pooledObject.ReleaseObject();
    }

    public override void Interact()
    {
        throw new System.NotImplementedException();
    }
}
