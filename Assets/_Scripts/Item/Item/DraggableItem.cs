using Ink.Parsed;
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

        transform.SetParent(parentTransform);
        elementImage.raycastTarget = true;

        //Manager.Instance.itemManager.CheckAbandonItem(element, CancelAbandonItem);

    }

    public void CancelAbandonItem()
    {

    }


}
