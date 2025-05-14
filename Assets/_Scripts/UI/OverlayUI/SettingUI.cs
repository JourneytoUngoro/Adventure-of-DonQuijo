#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{

    public Button confirmButton;
    public Button cancelButton;
    public Button temp_exitButton;

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

    public void OnClickExitButton()
    {
# if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
