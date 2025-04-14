using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
// 여러 객체에서 구조체처럼 쓰인다
public class InventoryData
{
    public Item[] items;
    public int capacity;
    public int coins;
}
