#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{

    public Button confirmButton;
    public Button cancelButton;

    private PopupUI popup;
    private RectTransform contentRect;

    private void Start()
    {
        popup = GetComponent<PopupUI>();
        popup.SetOnActivated(OnPopupOpened);
        contentRect = FindRectTransform(transform, "Content"); // Should check object's hierarchy 

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

    public void OnPopupOpened()
    {
        Vector2 currentAnchoredPos = contentRect.anchoredPosition;
        currentAnchoredPos.y = 0;
        contentRect.anchoredPosition = currentAnchoredPos;
    }

    private RectTransform FindRectTransform(Transform parent, string objectName)
    {
        foreach (Transform current in parent.GetComponentsInChildren<Transform>())
        {
            if (current.gameObject.name == objectName)
            {
                return current.GetComponent<RectTransform>();
            }
        }
        return null;

    }
}
