using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInventory : MonoBehaviour
{
    [SerializeField] InventoryView view;
    [SerializeField] Inventory inventory;
    InventoryData inventoryData;

    [SerializeField] static List<ItemDetails> testStartItems;


    public static List<ItemDetails> LoadInventory()
    {

        return testStartItems;
    }

}
