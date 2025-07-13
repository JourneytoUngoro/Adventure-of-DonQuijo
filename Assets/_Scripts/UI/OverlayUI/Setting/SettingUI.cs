#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{

    public Button confirmButton;
    public Button cancelButton;

    PopupUI popup;

    private void Start()
    {
        popup = GetComponent<PopupUI>();

        SetEvent();
    }

    void SetEvent()
    {
        confirmButton.onClick.AddListener(OnClickConfirmButton);
        cancelButton.onClick.AddListener(OnClickCancelButton);
    }

    public void OnClickConfirmButton()
    {
        popup.HideUI();
    }

    public void OnClickCancelButton()
    {
        popup.HideUI();
    }


}
