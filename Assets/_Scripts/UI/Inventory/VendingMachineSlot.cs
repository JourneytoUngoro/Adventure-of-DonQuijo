using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VendingMachineSlot : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int slotId;

    public int itemId;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemPrice;
    public TextMeshProUGUI itemInfo;

    public Image itemImage;
    [HideInInspector] public Item item;

    private Image backgroundImage;

    private PanelScaler scaler;
    private VendingMachinePopupUI vmUI;
    private PopupUI vmPopup;

    private void Start()
    {
        scaler = GetComponent<PanelScaler>();
        vmUI = GetComponentInParent<VendingMachinePopupUI>();
        vmPopup = vmUI.GetComponent<PopupUI>();

        backgroundImage = GetComponentInParent<Image>();

        GetComponent<Button>().onClick.AddListener(OnClickVMSlot);

        Debug.Assert(vmUI != null, "VendingMachinePopupUI is null!");
    }


    public void SetItemData(Item item)
    {
        this.item = item;
        itemId = item.id;
        itemName.text = item.details.label;
        itemInfo.text = item.details.description;
        itemPrice.text = "$ "+ item.details.price.ToString();

        itemImage.sprite = item.details.icon;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!vmPopup.isOpened ) return;

        vmUI.SetSelectedSlot(this);
        ChangeSelectedState(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ChangeSelectedState(false);
    }

    public void ChangeSelectedState(bool chosen)
    {
        if (chosen)
        {
            scaler.OnSelectedState();
        }
        else
        {
            scaler.OnUnSelectedState();
        }
    }

    public void OnClickVMSlot()
    {
        vmUI.ShowPurchaseGuide();
        Debug.Log("on click vm slot");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer Enter");
        ChangeSelectedState(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer Exit");
        ChangeSelectedState(false);
    }
}
