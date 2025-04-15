using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendingMachineSlot : MonoBehaviour
{
    public int itemId;
    public TextMeshProUGUI itemName;
    public Button getButton;

    public void SetItemData(int id, string name)
    {
        itemId = id;
        itemName.text = name;
    }

}
