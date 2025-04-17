using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    [field: SerializeField] public int id;
    public ItemDetails details;
    public int quantity;
    public int nowUseCount;

    public Item(ItemDetails details, int quantity = 1)
    {
        this.id = details.id;
        this.details = details;
        this.quantity = quantity;
        nowUseCount = 0;
    }


}
