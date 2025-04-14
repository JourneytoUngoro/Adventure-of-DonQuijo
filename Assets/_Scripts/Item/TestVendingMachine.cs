using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestVendingMachine : MonoBehaviour
{
    public GameObject VendingMachinePanel;

    ClickableObject clickable;
    PopupUI popup;

    private void Start()
    {
        clickable = GetComponent<ClickableObject>();
        clickable.onClick += ShowVendinMachinePanel;

        popup = VendingMachinePanel.GetComponent<PopupUI>();
    }

    private void ShowVendinMachinePanel()
    {
        Debug.Log("클릭됨");
        popup.ShowUI();
    }


}
