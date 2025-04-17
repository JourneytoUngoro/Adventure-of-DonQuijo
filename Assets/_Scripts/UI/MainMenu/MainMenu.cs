using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    // TODO : odin 패키지 설치 후 인스펙터 정리 
    [Header("Main Menu Buttons")]
    [SerializeField] Button newGameButton;
    [SerializeField] Button loadGameButton;
    [SerializeField] Button creditButton;
    [SerializeField] Button exitGameButton;

    [Header("Main Menu Panels")]
    public GameObject newGamePanel;
    public GameObject loadGamePanel;
    public GameObject creditPanel;
    public GameObject exitGamePanel;

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
        newGamePopup = newGamePanel.GetComponent<PopupUI>();
        loadGamePopup = loadGamePanel.GetComponent<PopupUI>();
        creditPopup = creditPanel.GetComponent<PopupUI>();
        exitGamePopup = exitGamePanel.GetComponent<PopupUI>();

        newGameButton.onClick.AddListener(OnClickNewGameButton);
        loadGameButton.onClick.AddListener(OnClickLoadGameButton);
        creditButton.onClick.AddListener(OnClickCreditButton);
        exitGameButton.onClick.AddListener (OnClickExitGameButton);
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


}
