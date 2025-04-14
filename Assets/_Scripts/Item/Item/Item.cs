using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    [field: SerializeField] public int id;
    [field: SerializeField] public int detailsId;
    public ItemDetails details;
    public int quantity;

    public Item(ItemDetails details, int quantity = 1)
    {
        id = details.id;
        this.detailsId = details.id;
        this.details = details;
        this.quantity = quantity;
    }


}
