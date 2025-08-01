using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [ShowInInspector]
    public static Dictionary<UIType, UIBase> startingUIDictionary = new Dictionary<UIType, UIBase>();

    private PopupUI popupPrefab;
    private TextInfoUI textInfoPrefab;
    private ImageUI imagePrefab;

    public Stack<PopupUI> activatedPopups = new Stack<PopupUI>();

    private bool toggleMenuPressed;
    private bool popupOpened;

    private TextInfoUI currentTextInfoUI;
    private ImageUI clickBlockImageUI;


    #region UI Object Pool
    public UIObjectPool<PopupUI> popupPool;
    public UIObjectPool<TextInfoUI> textInfoPool;
    public UIObjectPool<ImageUI> imagePool;

    int objectCount = 2;
    #endregion

    private Transform uiCanvas; // UI Object가 표시될 전용 캔버스 
    private Transform pool;

    private void Awake()
    {
        uiCanvas = GameObject.Find("Overlay Canvas")?.transform;
        pool = GameObject.Find("Pooled Objects")?.transform;
        InCaseTestScene();

        toggleMenuPressed = false;
        popupOpened = false;

        currentTextInfoUI = null;

        LoadUIPrefabs();
        RegisterUIObjects();
        CreatePool();
    }

    private void Update()
    {
        toggleMenuPressed = Manager.Instance.inputHandler.toggleMenuPressed;

        if (toggleMenuPressed)
        {
            EscPressed();
        }

        IsOpenedPopup();

        if (popupOpened && Manager.Instance.inputHandler.IsCharacterControlEnabled())
        {
            Manager.Instance.inputHandler.SetCharacterControlEnabled(false);
        }
/*        else if (!popupOpened && !Manager.Instance.inputHandler.IsCharacterControlEnabled())
        {
            Manager.Instance.inputHandler.SetCharacterControlEnabled(true);
        }*/
    }

    private void LoadUIPrefabs()
    {
        if (pool.transform.childCount >= 4) return;

        popupPrefab = Resources.Load<PopupUI>("Prefabs/UI/PopupPrefab");
        textInfoPrefab = Resources.Load<TextInfoUI>("Prefabs/UI/TextInfoPrefab");
        imagePrefab = Resources.Load<ImageUI>("Prefabs/UI/ImagePrefab");

        clickBlockImageUI = Object.Instantiate(Resources.Load<GameObject>("Prefabs/UI/ClickBlocker")).GetComponent<ImageUI>();
        clickBlockImageUI.transform.SetParent(uiCanvas, false);

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
        if (pool.transform.childCount >= 4) return; 
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

    public T GetUI<T>(UIType type) where T : UIBase
    {
        return startingUIDictionary[type] as T;
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
            // Debug.Log($"activated loadPopup : {activatedPopups.Count}, {activatedPopups.Peek().name}");
            activatedPopups.Peek().HideUI();
        }
        else
        {
            if (IsIngame())
            {
                // InGame -> Pause Popup Menu
                startingUIDictionary[UIType.pausePopup].ShowUI();
            }
            else
            {
                // else, -> Setting Popup
                startingUIDictionary[UIType.settingPopup].ShowUI();
            }
        }
    }

    public bool IsIngame() => !SceneManager.GetActiveScene().name.Equals("MainMenu");

    public void IsOpenedPopup()
    {
        if (activatedPopups.Count > 0)
        {
            popupOpened = true;
        }
        else
        {
            popupOpened = false;
            clickBlockImageUI?.HideUI();
         }
        // Debug.Log(Manager.Instance.uiManager.activatedPopups.Count + " popups activated");

    }

    // 최상단 팝업 아래 이미지를 깔아 이외 클릭을 막는 함수 BeforeShowPopupUI(), AfterHidePopupUI()
    public void BeforeShowPopupUI()
    {
        SetClockBlockerTransform(activatedPopups.Peek().transform);
    }

    public void AfterHidePopupUI()
    {
        SetClockBlockerTransform(activatedPopups.Peek().transform);
    }


    public void OpenTextInfoUI(TextInfoUI textInfo) => currentTextInfoUI = textInfo;

    public bool CheckCurrentTextInfo(TextInfoUI textInfo) => ReferenceEquals(currentTextInfoUI, textInfo);

    public void HideCurrentTextInfoUI()
    {
        if (currentTextInfoUI != null)
        {
            // just set alpha value to 0
            currentTextInfoUI.GetComponent<CanvasGroup>().alpha = 0f;
        }
        currentTextInfoUI = null;
    }

    private void SetClockBlockerTransform(Transform topPopup)
    {
        Transform parent = topPopup.transform.parent;
        int siblingIndex = topPopup.transform.GetSiblingIndex();

        // Positioned directly below the peek loadPopup
        clickBlockImageUI.ShowUI();

        clickBlockImageUI.transform.SetParent(parent);
        clickBlockImageUI.transform.SetSiblingIndex(siblingIndex - 1);
        Debug.Log($"clickblocker should be index {siblingIndex}");
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Implement logic to initialize necessary UI elements

        if (clickBlockImageUI == null || clickBlockImageUI.transform.parent != uiCanvas || clickBlockImageUI.transform.parent == null)
        {
            // If clickBlockImage is either not on the "Overlay Cavas" or destroyed
            clickBlockImageUI = Object.Instantiate(Resources.Load<GameObject>("Prefabs/UI/ClickBlocker")).GetComponent<ImageUI>();
            clickBlockImageUI.transform.SetParent(uiCanvas, false);
        }
    }

    #region Test 이후 삭제
    void InCaseTestScene()
    {
        if (uiCanvas == null)
        {
            Debug.LogWarning("UIManager error! uiCanvas can't find proper transform");
            
            Transform tmpTransform = FindObjectOfType<Canvas>().transform;
            uiCanvas = tmpTransform;
        }

        if (pool == null)
        {
            Debug.LogWarning("UIManager error! pool can't find proper transform");

            GameObject pooledObject = new GameObject("Pooled Objects");
            pool = pooledObject.transform;
            pooledObject.transform.SetParent(uiCanvas);
        }
    }
    #endregion


}
