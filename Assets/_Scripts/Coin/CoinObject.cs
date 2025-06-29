using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinObject : InteractBase
{
    public LayerMask playerMask;

    [field: SerializeField] public int minValue { get; private set; }
    [field: SerializeField] public int maxValue { get; private set; }
    [field: SerializeField] public int value {  get; private set; }

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

        SetCoinInfos();
    }


    public void SetCoinInfos()
    {
        this.value = Random.Range(minValue, maxValue + 1);
        isChasingPlayer = false;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (isChasingPlayer) { return; }

        StartCoroutine(NearToPlayer());
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

        Manager.Instance.soundManager.PlaySoundFXClip("coinCollectSFX", transform);
        Manager.Instance.itemManager.UpdateCoinAmount(this.value);

        // Always return this object to the pool when done.
        pooledObject.ReleaseObject();        
    }

    public override void Interact()
    {
        throw new System.NotImplementedException();
    }
}
