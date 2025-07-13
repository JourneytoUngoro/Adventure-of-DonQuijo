using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
   public static Dictionary<UIType, UIBase> startingUIDictionary = new Dictionary<UIType, UIBase>();

    // TODO : 테스트, 폴더 통합 이후 Load 하는 방식으로 바꾸기
    public PopupUI popupPrefab;
    public TextInfoUI textInfoPrefab;
    public ImageUI imagePrefab;

    public Stack<PopupUI> activatedPopups = new Stack<PopupUI>();

    private bool toggleMenuPressed;
    private bool popupOpened;

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
        uiCanvas = GameObject.Find("Overlay Canvas")?.transform;
        pool = GameObject.Find("Pooled Objects")?.transform;

        InCaseTestScene();

        toggleMenuPressed = false;
        popupOpened = false;

        RegisterUIObjects();
        CreatePool();
    }

    private void Update()
    {
        toggleMenuPressed = Manager.Instance.inputHandler.toggleMenuPressed;

        if (toggleMenuPressed)
        {
            Debug.Log("Esc Key pressed");
            EscPressed();
        }

        IsOpenedPopup();

        if (popupOpened && Manager.Instance.inputHandler.IsCharacterControlEnabled())
        {
            Manager.Instance.inputHandler.SetCharacterControlEnabled(false);
        }
        else if (!popupOpened && !Manager.Instance.inputHandler.IsCharacterControlEnabled())
        {
            Manager.Instance.inputHandler.SetCharacterControlEnabled(true);
        }
        // Debug.Log("CharacterControl is enabled : " + Manager.Instance.inputHandler.IsCharacterControlEnabled());
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
        if (popupOpened)
        {
            // Debug.Log($"activated popup : {activatedPopups.Count}, {activatedPopups.Peek().name}");
            activatedPopups.Peek().HideUI();
        }
        else
        {
             startingUIDictionary[UIType.settingPopup].ShowUI();
        }
    }

    public void IsOpenedPopup()
    {
        if (activatedPopups.Count > 0)
        {
            popupOpened = true;
        }
        else 
        {
            popupOpened = false;
        }
        // Debug.Log(Manager.Instance.uiManager.activatedPopups.Count + " popups activated");

    }

    #region Test 이후 삭제
    void InCaseTestScene()
    {
        if (uiCanvas == null || pool == null)
        {
            Debug.LogWarning("UIManager error! uiCanvas or pool can't find proper transform");
            
            Transform tmpTransform = FindObjectOfType<Canvas>().transform;
            uiCanvas = tmpTransform;
            pool = tmpTransform;
        }
    }
    #endregion


}
