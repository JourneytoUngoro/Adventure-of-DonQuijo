using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : Slot<Item>
{
    [SerializeField] Sprite nullSprite;
    [SerializeField] TextMeshProUGUI quantityTMP;

    public override void AfterSwapElement(Slot<Item> slot1, Slot<Item> slot2)
    {
        base.AfterSwapElement(slot1, slot2);

        ItemManager.Instance.SwapItems(slot1.SlotId, slot2.SlotId);

/*        ItemSlot tmp1 = slot1 as ItemSlot;
        ItemSlot tmp2 = slot2 as ItemSlot;

        TextMeshProUGUI tmpTMP = tmp1.quantityTMP;

        tmp1.quantityTMP = tmp2.quantityTMP;
        tmp2.quantityTMP = tmpTMP;*/

    }


    public void SetItemToSlot(Item item)
    {
        if (item == null)
        {
            DraggableElement.elementImage.sprite = nullSprite;
            quantityTMP.text = string.Empty;
            DraggableElement.element = null;
        }
        else
        {
            DraggableElement.element = item;
            DraggableElement.element.id = item.id;
            DraggableElement.element.details = item.details;
            DraggableElement.elementImage.sprite = item.details.icon;

            quantityTMP.text = item.quantity.ToString();
            // Debug.Log($"{DraggableObject.element.details.name} now count {DraggableObject.element.quantity}, but tmp {quantityTMP.text}");
        }
    }
}
