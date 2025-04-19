using System;
using UnityEngine;


/*public enum ItemType
{
    LifePotion,
    IronFist,
    SwiftFeather,
    MemoryFragment,
    MedKit,
    None,
}
*/

[Serializable]
public class Item
{
    // public ItemType type;
    public int id;
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
