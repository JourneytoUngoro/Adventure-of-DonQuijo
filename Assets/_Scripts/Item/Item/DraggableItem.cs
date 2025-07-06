using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class DraggableItem : DraggableObject<Item>
{

    public override void InitElements()
    {
        Debug.Log("Init Elements");
        element = ItemDatabase.GetDetailsById(element.id).Create();
        elementImage = GetComponent<Image>();

        elementImage.sprite = element.details.icon;
    }

    public override void DroppedOutsideSlot()
    {
        base.DroppedOutsideSlot();
        Manager.Instance.itemManager.CheckAbandonItem(this, element);

    }

    public void ConfirmAbandonItem()
    {
        transform.SetParent(parentTransform);
        elementImage.raycastTarget = true;
    }

    public void CancelAbandonItem()
    {
        transform.SetParent(parentTransform);
        elementImage.raycastTarget = true;
    }


}
