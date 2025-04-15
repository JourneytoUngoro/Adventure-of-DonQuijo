using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendingMachinePopup : MonoBehaviour
{
    Transform items;
    VendingMachineSlot[] slots;

    private void Awake()
    {
        items = transform.Find("Items").transform;
        slots = items.GetComponentsInChildren<VendingMachineSlot>();

        SetUpItems();
    }

    void SetUpItems()
    {
        Debug.Log(ItemDatabase.totalItems + " (count)");
        for (int i = 1; i <= ItemDatabase.totalItems; i++)
        {
            Item item = ItemDatabase.GetDetailsById(i).Create();

            slots[i-1].SetItemData(item.id, item.details.name);

            slots[i-1].getButton.onClick.RemoveAllListeners();
            slots[i-1].getButton.onClick.AddListener( () => { PurchaseItem(item); });
        }

    }

    void PurchaseItem(Item item)
    {
        Manager.Instance.itemManager.PurchaseItem(item);
    }


}
