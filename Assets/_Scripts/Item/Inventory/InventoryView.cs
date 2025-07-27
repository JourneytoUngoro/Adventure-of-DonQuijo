using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class InventoryView : MonoBehaviour
{
    private GameObject inventory;

    private ItemSlot[] itemSlots;
    private TextMeshProUGUI coinTMP;
    private Transform canvasTransform;
    private Transform originalParentTransform;

    [Tooltip("Check if the player want to drop item")]
    public PopupUI guidePopup; // Ask the player if they want to discard the item by dragging it

    private TaskCompletionSource<bool> abandonDecision;

    private void Start()
    {
        guidePopup = transform.parent.Find("Guide Popup")?.GetComponent<PopupUI>();
        canvasTransform = GetComponentInParent<Canvas>().transform;
        originalParentTransform = transform.parent;
        Debug.Assert(guidePopup != null, "InventoryView.guidePopup is null!");
        Debug.Assert(canvasTransform != null, "Overaly Canvas trasform is null!");

        SetGuidePopupEvents();
    }

    private void OnEnable()
    {
        // Input must be enabled for event registration; restore previous state after setup
        bool enabled = Manager.Instance.inputHandler.IsUIControlEnabled();
        Manager.Instance.inputHandler.SetUIControlEnabled(true);
        Manager.Instance.inputHandler.controls.UIControl.Confirm.performed += ctx => OnClickGuidePopupConfrimBtn(); // Confirm - Z key
        Manager.Instance.inputHandler.controls.UIControl.Cancel.performed += ctx => OnClickGuidePopupConfrimBtn(); // Cancel - X key
        Manager.Instance.inputHandler.SetUIControlEnabled(enabled);
    }

    private void OnDisable()
    {
        // Input must be enabled for event registration; restore previous state after setup
        bool enabled = Manager.Instance.inputHandler.IsUIControlEnabled();
        Manager.Instance.inputHandler.SetUIControlEnabled(true);
        Manager.Instance.inputHandler.controls.UIControl.Confirm.performed -= ctx => OnClickGuidePopupConfrimBtn(); // Confirm - Z key
        Manager.Instance.inputHandler.controls.UIControl.Cancel.performed -= ctx => OnClickGuidePopupConfrimBtn(); // Cancel - X key
        Manager.Instance.inputHandler.SetUIControlEnabled(enabled);
    }

    public IEnumerator InitializeView()
    {
        // Coroutine used to delay UI initialization by one frame
        inventory = gameObject;
        itemSlots = gameObject.GetComponentsInChildren<ItemSlot>();
        coinTMP = transform.parent.Find("Coin Inventory").GetComponentInChildren<TextMeshProUGUI>();
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

    public async Task<bool> CheckAbandonItem(Item item)
    {
        guidePopup.SetPopupInfo($"{item.details.label} 아이템을\n버리시겠습니까?");
        guidePopup.ShowUI();
        guidePopup.transform.SetParent(canvasTransform);
        guidePopup.transform.SetAsLastSibling();

        abandonDecision = new TaskCompletionSource<bool>();        

        return await abandonDecision.Task;
    }

    private void SetGuidePopupEvents()
    {
        guidePopup.SetDynamicPopupEvent(OnClickGuidePopupConfrimBtn, OnClickGuidePopupCancelBtn);
    }

    private void OnClickGuidePopupConfrimBtn()
    {
        if (!guidePopup.isOpened) return;
        abandonDecision?.TrySetResult(true);
        guidePopup.HideUI();
        guidePopup.transform.SetParent(originalParentTransform);
    }

    private void OnClickGuidePopupCancelBtn()
    {
        if (!guidePopup.isOpened) return;

        abandonDecision?.TrySetResult(false);
        guidePopup.HideUI();
        guidePopup.transform.SetParent(originalParentTransform);
    }
}
