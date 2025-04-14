using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryView : MonoBehaviour
{
    public GameObject inventory;

    public ItemSlot[] itemSlots;
    public TextMeshProUGUI coinTMP;

    public IEnumerator InitializeView()
    {
        itemSlots = gameObject.GetComponentsInChildren<ItemSlot>();
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
