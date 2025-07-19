using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class VendingMachineController : InteractBase
{
    public GameObject VendingMachinePanel;

    private PopupUI popup;
    private VendingMachinePopupUI vmPopup;
    private TextInfoUI textUI;

    private float delayTime = 0.4f;

    private void Start()
    {
        popup = VendingMachinePanel.GetComponent<PopupUI>();
        vmPopup = VendingMachinePanel.GetComponent<VendingMachinePopupUI>();

        popup.SetOnHide(SetCanOpenGuideFalse);
    }

    public override void Interact()
    {
        ShowVendinMachinePanel();
    }

    private void ShowVendinMachinePanel()
    {
        popup.ShowUI();
        StartCoroutine(SetCanOpenGuide(true));
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (textUI == null)
        {
            textUI = Manager.Instance.uiManager.ShowDynamicTextInfo(new TextInfoData("Z를 눌러 상호작용하세요."));
            textUI.SetAnchoredPositioin(0, -400);
            textUI.ShowUI();
        }
        // Enable Outline or Interaction Image

    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        textUI?.HideUI();
        textUI = null;

        // Disable Outline or Interaction Image

    }

    private IEnumerator SetCanOpenGuide(bool openGuide)
    {
        yield return new WaitForSeconds(delayTime);

        vmPopup.canOpenGuide = openGuide;
    }

    private void SetCanOpenGuideFalse() => vmPopup.canOpenGuide = false;

}
