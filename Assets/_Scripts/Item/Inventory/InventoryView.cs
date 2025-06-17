using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InventoryView : MonoBehaviour
{
    GameObject inventory;

    ItemSlot[] itemSlots;
    TextMeshProUGUI coinTMP;

    public PopupUI guidePopup;

    private void Start()
    {
        guidePopup = transform.parent.Find("Guide Popup")?.GetComponent<PopupUI>();
        Debug.Assert(guidePopup != null, "InventoryView.guidePopup is null!");

        SetGuidePopupEvents();
    }

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

    public bool CheckAbandonItem(Item item)
    {
        guidePopup.SetPopupInfo($"{item.details.label} 아이템을 버리시겠습니까?");
        guidePopup.ShowUI();

        return true;
    }

    private void SetGuidePopupEvents()
    {
        guidePopup.SetDynamicPopupEvent(OnClickGuidePopupConfrimBtn, OnClickGuidePopupCancelBtn);
    }

    private void OnClickGuidePopupConfrimBtn()
    {
        // Manager.Instance.itemManager.
        guidePopup.HideUI();
    }

    private void OnClickGuidePopupCancelBtn()
    {
        guidePopup.HideUI();
    }
}
