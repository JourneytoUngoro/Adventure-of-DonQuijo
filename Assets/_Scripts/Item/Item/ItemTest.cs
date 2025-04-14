using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemTest : MonoBehaviour
{
    public TMP_InputField test_vending_machine;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            ItemManager.Instance.UseItem(0);
        }

        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            ItemManager.Instance.UseItem(1);
        }

        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            ItemManager.Instance.UseItem(2);
        }
    }


    public void OnClickAcquireButton()
    {
        string value = test_vending_machine.text;
        int index = Test_ChangeStringToInt(value);

        Item item = ItemDatabase.GetDetailsById(index).Create();

        ItemManager.Instance.AcquireItem(item);

    }

    public void OnClickPurchaseButton()
    {
        string value = test_vending_machine.text;
        int index = Test_ChangeStringToInt(value);

        Item item = ItemDatabase.GetDetailsById(index).Create();

        ItemManager.Instance.PurchaseItem(item);

    }

    public int Test_ChangeStringToInt(string value)
    {
        int index = 0;
        switch (value)
        {
            case "1":
                index = 1;
                break;
            case "2":
                index = 2;
                break;
            case "3":
                index = 3;
                break;
            case "4":
                index = 4;
                break;
            case "5":
                index = 5;
                break;
            default:
                index = 1;
                break;
        }

        return index;
    }

}
