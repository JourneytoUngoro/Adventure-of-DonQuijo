using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [field: SerializeField] public int stage { get; private set; }
    [field: SerializeField] public int minValue { get; private set; }
    [field: SerializeField] public int maxValue { get; private set; }
    [field: SerializeField] public int value {  get; private set; }

    public Sprite icon;
    public LayerMask playerMask;



    public void SetCoinValue()
    {
        this.value = Random.Range(minValue, maxValue + 1);
    }

    public void PlayerGetCoins()
    {
        // TODO : 인벤토리의 코인 목록에 추가되는 로직 
        Debug.Log($"Player Get {value} won");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 충돌 시 코인 획득
        if (((1 << collision.gameObject.layer) & playerMask) != 0)
        {
            PlayerGetCoins();
        }
    }

}
