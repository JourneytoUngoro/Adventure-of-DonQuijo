using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemUsageData
{
    public Item[] itemUsageData;

    public ItemUsageData()
    {
        itemUsageData = new Item[ItemDatabase.totalItems];
        for (int i = 1; i <= ItemDatabase.totalItems; i++)
        {
            itemUsageData[i-1] = ItemDatabase.GetDetailsById(i).Create(0);
        }
    }
}
