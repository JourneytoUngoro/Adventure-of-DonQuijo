using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VendingMachineSlot : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool printDebug = true;


    public int slotId;

    public int itemId;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemPrice;
    public TextMeshProUGUI itemInfo;

    public Image itemImage;
    [HideInInspector] public Item item;

    private Image backgroundImage;

    public VendingMachineSlotEffect effect;
    private VendingMachinePopupUI vmUI;
    private PopupUI vmPopup;

    private Image buttonSlotImage;
    private Color normalColor;
    private Color highlightColor;

    private float originalFontSize;

    public bool canPurchase;

    private void Start()
    {
        if (effect == null) { effect = GetComponent<VendingMachineSlotEffect>(); }
        vmUI = GetComponentInParent<VendingMachinePopupUI>();
        vmPopup = vmUI.GetComponent<PopupUI>();

        backgroundImage = GetComponentInParent<Image>();

        GetComponent<Button>().onClick.AddListener(OnClickVMSlot);
        buttonSlotImage = GetComponent<Image>();
        normalColor = GetComponent<Button>().colors.normalColor;
        highlightColor = GetComponent<Button>().colors.highlightedColor;
        originalFontSize = itemPrice.fontSize;
        canPurchase = true;

        if (effect == null)
        {
            effect = GetComponent<VendingMachineSlotEffect>();
        }

        Debug.Log(effect == null ? "null" : "not null");
        Debug.Log(vmUI == null ? "null" : "not null");
        Debug.Log(vmPopup == null ? "null" : "not null");
    }


    public void SetItemData(Item item)
    {
        this.item = item;
        itemId = item.id;
        itemName.text = item.details.label;
        itemInfo.text = item.details.description;
        itemImage.sprite = item.details.icon;

        if (effect == null) { Debug.LogError("itemEffect is null!"); effect = GetComponent<VendingMachineSlotEffect>(); }

        Debug.Log($"{(Manager.Instance == null ? "Manager null" : "Manager not null")}");
        Debug.Log($"아이템 구매 여부 : {Manager.Instance.itemManager.CanPurchaseItem(item)}");

        if (/*Manager.Instance.itemManager.CanPurchaseItem(item)*/ true)
        {
            itemPrice.text = item.details.price.ToString();
            itemPrice.fontSize = originalFontSize;
            gameObject.GetComponent<Button>().enabled = true;
            canPurchase = true;
        }
        else
        {
            itemPrice.text = "구매 불가";
            itemPrice.fontSize = 12f;
            gameObject.GetComponent<Button>().enabled = false;
            canPurchase = false;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!vmPopup.isOpened ) return;

        vmUI.SetSelectedSlot(this);
        ChangeSelectedState(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (!vmPopup.isOpened) return;

        ChangeSelectedState(false);
    }

    public void ChangeSelectedState(bool chosen)
    {

        Debug.Log(effect == null ? "effect null" : "effect not null");

        if (chosen && canPurchase)
        {
            effect?.OnSelectedState();
            buttonSlotImage.color = highlightColor;
        }
        else
        {
            effect?.OnUnSelectedState();
            buttonSlotImage.color = normalColor;
        }
    }

    public void OnClickVMSlot()
    {
        vmUI.ShowPurchaseGuide();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!vmPopup.isOpened) return;

        vmUI.SetSelectedSlot(this);
        ChangeSelectedState(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!vmPopup.isOpened) return;

        ChangeSelectedState(false);
    }
}
