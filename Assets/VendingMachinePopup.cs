using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendingMachinePopup : MonoBehaviour
{
    Transform items;
    VendingMachineSlot[] slots;
    PopupUI popupUI;

    private void Awake()
    {
        items = transform.Find("Items").transform;
        slots = items.GetComponentsInChildren<VendingMachineSlot>();
        popupUI = GetComponent<PopupUI>();

        SetUpItems();
        SetEvents();
    }

    // 자판기에 아이템 등록 
    void SetUpItems()
    {
        for (int i = 1; i <= ItemDatabase.totalItems; i++)
        {
            Item item = ItemDatabase.GetDetailsById(i).Create();

            slots[i-1].SetItemData(item.id, item.details.label);

            slots[i-1].getButton.onClick.RemoveAllListeners();
            slots[i-1].getButton.onClick.AddListener( () => { PurchaseItem(item); });
        }

    }

    void PurchaseItem(Item item)
    {
        Debug.Assert(item != null, "vending machine item null!");
        Manager.Instance.itemManager.PurchaseItem(item);
    }

    void SetEvents()
    {
        popupUI.confirmButton.onClick.AddListener(() => { popupUI.HideUI(); }); 
    }


}
