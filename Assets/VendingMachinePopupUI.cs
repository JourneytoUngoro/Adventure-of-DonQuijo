using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VendingMachinePopupUI : MonoBehaviour
{
    private Transform items;
    private VendingMachineSlot[] slots;
    private PopupUI vmPopup;
    public PopupUI guidePopup;

    public VendingMachineSlot selectedSlot { get; private set; }

    public bool canOpenGuide;

    private void Awake()
    {
        items = transform.Find("Items").transform;
        slots = items.GetComponentsInChildren<VendingMachineSlot>();
        vmPopup = GetComponent<PopupUI>();

        canOpenGuide = false;
    }

    private void Start()
    {
        SetUpItems();
        SetEvents();
        AddGuidePopupListener();
    }

    private void Update()
    {
        if (!vmPopup.isOpened) return;

        KeepSelectedSlot();

        if (Manager.Instance.inputHandler.confirmInputPressed && canOpenGuide) // UI Confirm Pressed
        {
            if (!guidePopup.isOpened)
            {
                ShowPurchaseGuide();
            }
            else
            {
                OnClickGuideConfirmBtn();
            }
        }

        if (Manager.Instance.inputHandler.cancelInputPressed && guidePopup.isOpened) // UI Cancel pressed
        {
            OnClickGuideCancelBtn();
        }
    }

    // 자판기에 아이템 등록 
    private void SetUpItems()
    {
        for (int i = 1; i <= ItemDatabase.totalItems; i++)
        {
            Item item = ItemDatabase.GetDetailsById(i).Create();

            slots[i - 1].SetItemData(item);
            slots[i - 1].slotId = i - 1;

        }
        SetSelectedSlot(slots[0]);
        StartSelectSlotUI(selectedSlot);
    }

    // UI 이벤트 등록 
    private void SetEvents()
    {
        vmPopup.confirmButton.onClick.RemoveAllListeners();

        vmPopup.confirmButton.onClick.AddListener(() => { vmPopup.HideUI(); });

        vmPopup.SetOnShow(() => StartSelectSlotUI(slots[0]));

        guidePopup.SetOnShow(() => { EventSystem.current.sendNavigationEvents = false; });
        guidePopup.SetOnHide(() => { EventSystem.current.sendNavigationEvents = true; });
    }

    private void PurchaseItem(Item item)
    {
        Debug.Assert(item != null, "vending machine item null!");
        Manager.Instance.itemManager.PurchaseItem(item);
    }


    public void SetSelectedSlot(VendingMachineSlot slot)
    {
        selectedSlot = slot;
    }

    public void StartSelectSlotUI(VendingMachineSlot slot)
    {
        EventSystem.current.SetSelectedGameObject(null);

        SetSelectedSlot(slot);
        StartCoroutine(SelectSlotUI(selectedSlot));
    }

    IEnumerator SelectSlotUI(VendingMachineSlot selectedSlot)
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(selectedSlot.gameObject);
    }

    // 적어도 하나의 슬롯은 언제나 활성화
    private void KeepSelectedSlot()
    {
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
        if (selectedObject == null || !selectedObject.transform.IsChildOf(items))
        {
            StartSelectSlotUI(selectedSlot);
        }
    }

    public void AddGuidePopupListener()
    {
        PopupData data = new PopupData
        (
            string.Empty, $"아이템을\n구매하시겠습니까?", "예 (Z)", "아니오 (X)"
        );

        guidePopup.SetDynamicPopup(data);
        guidePopup.SetDynamicPopupEvent(OnClickGuideConfirmBtn, OnClickGuideCancelBtn);
    }

    private void ShowPurchaseGuide()
    {
        guidePopup.SetPopupInfo($"{selectedSlot.item.details.label} 아이템을 \n구매하시겠습니까?");
        guidePopup.ShowUI();
    }

    private void OnClickGuideConfirmBtn()
    {
        PurchaseItem(selectedSlot.item);
        guidePopup?.HideUI();
    }

    private void OnClickGuideCancelBtn()
    {
        guidePopup?.HideUI();
    }
}
