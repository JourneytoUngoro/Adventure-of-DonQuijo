using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
   public static Dictionary<UIType, UIBase> startingUIDictionary = new Dictionary<UIType, UIBase>();

    // TODO : 테스트, 폴더 통합 이후 Load 하는 방식으로 바꾸기
    public PopupUI popupPrefab;
    public TextInfoUI textInfoPrefab;
    public ImageUI imagePrefab;

    public Stack<PopupUI> activatedPopups = new Stack<PopupUI>();

    #region UI Object Pool
    public UIObjectPool<PopupUI> popupPool;
    public UIObjectPool<TextInfoUI> textInfoPool;
    public UIObjectPool<ImageUI> imagePool;

    int objectCount = 2;
    #endregion

    Transform uiCanvas; // UI Object가 표시될 전용 캔버스 
    Transform pool; 
    GameObject clickBlocker; // TODO : UI 활성화 시 클릭 막는 로직 추가 

    private void Awake()
    {
        uiCanvas = GameObject.Find("UICanvas").transform;
        pool = GameObject.Find("Pooled Objects").transform;

        RegisterUIObjects();
        CreatePool();
    }

    private void RegisterUIObjects()
    {
        UIBase[] startingUIs = uiCanvas.GetComponentsInChildren<UIBase>();

        foreach (UIBase ui in startingUIs)
        {
            if (!startingUIDictionary.ContainsKey(ui.type))
            {
                startingUIDictionary.Add(ui.type, ui);
            }
        }
    }

    void CreatePool()
    {
        popupPool = new UIObjectPool<PopupUI>(popupPrefab, objectCount, pool);
        textInfoPool = new UIObjectPool<TextInfoUI>(textInfoPrefab, objectCount, pool);
        imagePool = new UIObjectPool<ImageUI>(imagePrefab, objectCount, pool);
    }

    /// <summary>
    /// UIBase로 반환되므로 캐스팅이 필요하다 
    /// </summary>
    public UIBase GetUI(UIType type)
    {
        return startingUIDictionary[type];
    }

    public PopupUI ShowDynamicPopup(PopupData data)
    {
        PopupUI popup = popupPool.Get();
        popup.transform.SetParent(uiCanvas);
        popup.type = UIType.DynamicPopup;
        return popup.SetDynamicPopup(data);
    }

    public TextInfoUI ShowDynamicTextInfo(TextInfoData data)
    {
        TextInfoUI textInfo = textInfoPool.Get();
        textInfo.transform.SetParent(uiCanvas);
        textInfo.type = UIType.DynamicTextInfo;
        return textInfo.SetDynamicTextInfo(data);
    }

    public ImageUI ShowDynamicImage(UnityEngine.UI.Image data)
    {
        ImageUI image = imagePool.Get();
        image.transform.SetParent(uiCanvas);
        image.type = UIType.DynamicImage;
        return image.SetDynamicImage(data);
    }

    public void AddStartingUIs(UIType type, UIBase ui)
    {
        if (!startingUIDictionary.ContainsKey(type))
        {
            startingUIDictionary.Add(type, ui);
        }
    }

    public void RemoveStartingUIs(UIType type)
    {
        if (startingUIDictionary.ContainsKey(type))
        {
            startingUIDictionary.Remove(type);
        }

    }

    public void EscPressed()
    {
        if (activatedPopups.Count > 0)
        {
            Debug.Log($"activated popup : {activatedPopups.Count}");
            activatedPopups.Peek().HideUI();
        }
        else
        {
            startingUIDictionary[UIType.MainPopup].ShowUI();
        }
    }

}
