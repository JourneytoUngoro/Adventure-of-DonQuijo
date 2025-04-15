using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryView : MonoBehaviour
{
    GameObject inventory;

    ItemSlot[] itemSlots;
    TextMeshProUGUI coinTMP;

    public IEnumerator InitializeView()
    {
        inventory = gameObject;
        itemSlots = gameObject.GetComponentsInChildren<ItemSlot>();
        coinTMP = transform.parent.Find("Coin").GetComponentInChildren<TextMeshProUGUI>();
        yield return null;
    }

    public void RefreshSlots(int index, Item item)
    {
        itemSlots[index].SetItemToSlot(item);
    }

    public void RefreshCoins(int amount)
    {
        coinTMP.text = $"coin : {amount}";
    }

}
