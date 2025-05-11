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
}
