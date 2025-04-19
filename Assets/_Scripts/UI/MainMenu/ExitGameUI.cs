using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExitGameUI : MonoBehaviour
{
    PopupUI popup;
    private void Awake()
    {
        popup = GetComponent<PopupUI>();
    }

    private void Start()
    {
        popup.SetDynamicPopupEvent(OnClickConfirmButton, OnClickCancelButton);  
    }

    void OnClickConfirmButton()
    {
# if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void OnClickCancelButton()
    {
        popup.HideUI();
    }
}
