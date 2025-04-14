using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VendingMachinePopup : MonoBehaviour
{
    Button[] items;

    private void Awake()
    {
        items = GetComponentsInChildren<Button>();
    }

    void SetUpItems()
    {
        for (int i = 0; i < ItemDatabase.totalItems; i++)
        {
            Item item = ItemDatabase.GetDetailsById(i).Create();


        }

    }
}
