using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private Button temp;
    // Variable names in Scripts            // Object names in Scene Hierarchy
    public Transform buttonsTransform;  // Choice Buttons

    private Button continueButton;       // Continue Button
    private Button settingPopupButton; // Setting Popup Button
    private Button saveButton;            // Save Button
    private Button mainMenuButton;      // Main Menu Button

    private PopupUI pauseMenuPopup;
    private PopupUI guidePopup;

    private void Start()
    {
        InitializeReferences();
        RegisterAcivatedEvent();
        RegisterButtonEvents();
    }

    private void InitializeReferences()
    {
        pauseMenuPopup = GetComponent<PopupUI>();

        if (buttonsTransform == null)
        {
            buttonsTransform = transform.Find("Choice Buttons").GetComponent<Transform>();
        }

        continueButton = buttonsTransform.Find("Continue Button").GetComponent<Button>();
        settingPopupButton = buttonsTransform.Find("Setting Popup Button").GetComponent<Button>();
        saveButton = buttonsTransform.Find("Save Button").GetComponent<Button>();
        mainMenuButton = buttonsTransform.Find("Main Menu Button").GetComponent<Button>();
    }

    private void RegisterAcivatedEvent()
    {
        pauseMenuPopup.SetOnShow(OnPauseMenuActivated);
        pauseMenuPopup.SetOnHide(OnPaueMenuInactivated);
    }

    private void OnPauseMenuActivated()
    {
        // Prevent saving during cutscenes
        saveButton.gameObject.SetActive(!SceneManager.GetActiveScene().name.EndsWith("Cutscene"));
        Manager.Instance.gameManager.PauseGame();
    }

    private void OnPaueMenuInactivated()
    {
        Manager.Instance.gameManager.ResumeGame();
    }

    private void RegisterButtonEvents()
    {
        continueButton.onClick.RemoveAllListeners();
        settingPopupButton.onClick.RemoveAllListeners();
        saveButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.RemoveAllListeners();

        continueButton.onClick.AddListener(OnClickContinueButton);
        settingPopupButton.onClick.AddListener(OnClickSettingPopupButton);
        saveButton.onClick.AddListener(OnClickSaveButton);
        mainMenuButton.onClick.AddListener(OnClickMainMenuButton);
    }

    private void OnClickContinueButton()
    {
        pauseMenuPopup.HideUI();
    }

    private void OnClickSettingPopupButton()
    {
        Manager.Instance.uiManager.GetUI(UIType.settingPopup).ShowUI();
    }

    private void OnClickSaveButton()
    {
        Manager.Instance.gameManager.PauseGame();
        SaveCurrentGame();
    }

    private void OnClickMainMenuButton()
    {
        Debug.Log("on click main menu button!");
        guidePopup = Manager.Instance.uiManager.ShowDynamicPopup(
            new PopupData("", "게임을 저장하고\n메인 화면으로 돌아가시겠습니까?", "", ""));

        // set event
        guidePopup.SetDynamicPopupEvent(ConfirmReturnToMainMenu, CancelReturnToMainMenu);
        guidePopup.ShowUI();
    }

    private void SaveCurrentGame()
    {
        bool saved = Manager.Instance.dataManager.SaveGame();
        TextInfoUI guideText;
        if (saved)
        {
            guideText = Manager.Instance.uiManager.ShowDynamicTextInfo(
                new TextInfoData("저장에 성공했습니다."));
        }
        else
        {
            guideText = Manager.Instance.uiManager.ShowDynamicTextInfo(
                new TextInfoData("저장에 실패했습니다."));
        }
        guideText.SetAnchoredPosition(0, -400); // bottom of screen
        guideText.ShowAndHideUI(2f);

    }

    private void ConfirmReturnToMainMenu()
    {
        SaveCurrentGame();

        // close popup
        guidePopup.HideUI();
        pauseMenuPopup.HideUI();

        // Scene Transition
        Manager.Instance.sceneTransitionManager.SceneTransition(new SceneField("MainMenu"), true, true);

        guidePopup = null;
    }

    private void CancelReturnToMainMenu()
    {
        // close popup
        guidePopup.HideUI();
        pauseMenuPopup.HideUI();

        guidePopup = null;
    }
}