using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Details", menuName = "Scriptable Object/Item/ItemDetails")]

public class ItemDetails : ScriptableObject
{
    [field: SerializeField] public int id { get; private set; }
    [field: SerializeField] public string label { get; private set; }
    [field: SerializeField] public int maxStack { get; private set; }
    [field: SerializeField] public int maxOverlap { get; private set; }
    [field: SerializeField] public string description { get; private set; }
    [field: SerializeField] public int price { get; private set; }
    [field: SerializeField] public ItemEffect effect { get; private set; }

    [field : SerializeField] public Sprite icon;

    public Item Create(int quantity = 1)
    {
        return new Item(this, quantity); 
    }

    public void UseItem(Player target)
    {
        if (effect != null)
        {
            effect.ApplyEffect(target);
        }
    }


}
