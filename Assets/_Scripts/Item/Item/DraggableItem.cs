using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;


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
