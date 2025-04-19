using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    #region UI 오브젝트 변수

    // TODO : odin 패키지 설치 후 인스펙터 정리 
    [Header("Main Menu Buttons")]
    public Button newGameButton;
    public Button loadGameButton;
    public Button creditButton;
    public Button exitGameButton;
    public Button settingButton;

    [Header("Main Menu Panels")]
    public GameObject newGamePanel;
    public GameObject loadGamePanel;
    public GameObject creditPanel;
    public GameObject exitGamePanel;

    #endregion

    private PopupUI newGamePopup;
    private PopupUI loadGamePopup;
    private PopupUI creditPopup;
    private PopupUI exitGamePopup;

    private void Awake()
    {
        InitalizeMainMenu();
    }

    void InitalizeMainMenu()
    {
        // Popup 스크립트 할당
        newGamePopup = newGamePanel.GetComponent<PopupUI>();
        loadGamePopup = loadGamePanel.GetComponent<PopupUI>();
        creditPopup = creditPanel.GetComponent<PopupUI>();
        exitGamePopup = exitGamePanel.GetComponent<PopupUI>();

        // 이벤트 할당
        newGameButton.onClick.AddListener(OnClickNewGameButton);
        loadGameButton.onClick.AddListener(OnClickLoadGameButton);
        creditButton.onClick.AddListener(OnClickCreditButton);
        exitGameButton.onClick.AddListener(OnClickExitGameButton);
        settingButton.onClick.AddListener(OnClickSettingButton);
    }


    public void OnClickNewGameButton()
    {
        newGamePopup.ShowUI();
    }

    public void OnClickLoadGameButton()
    {
        loadGamePopup.ShowUI();
    }

    public void OnClickCreditButton()
    {
        creditPopup.ShowUI();
    }

    public void OnClickExitGameButton()
    {
        exitGamePopup.ShowUI();
    }

    public void OnClickSettingButton()
    {
        Manager.Instance.uiManager.GetUI(UIType.settingPopup).ShowUI();
    }


}
