using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 씬에 존재하는 Item 오브젝트
public class ItemObject : MonoBehaviour
{
    // ItemObject의 데이터
    public Item item { get; set; }


    [SerializeField] private LayerMask playerLayer;
    
    public void SetItem(Item item)
    {
        this.item = item;
    }

    public ItemObject Get()
    {
        return this;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            // TODO : 인벤토리에 추가하는 로직
            // 일단은 바로 사용한다
            item.details.UseItem(collision.GetComponent<Player>());
        }
    }



}
