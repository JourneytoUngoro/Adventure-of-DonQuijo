using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    public Button stage1Button;
    public Button mainMenuButton;

    private PopupUI gameOverPopup;

    private void Start()
    {
        gameOverPopup = GetComponent<PopupUI>();

        if (stage1Button == null) transform.Find("Stage1 Button").GetComponent<Button>();
        if (mainMenuButton == null) transform.Find("Main Button").GetComponent<Button>();

        AddListenerToButtons();
    }

    public void HasMemoryFragment()
    {
        int index = Manager.Instance.itemManager.HasItem("기억의 조각");

        if (index != -1) // player has mf
        {
            bool success = Manager.Instance.itemManager.UseItem(index);
            Player.Instance.ReviveWithMemoryFragment();
        }
        else
        {
            gameOverPopup.ShowUI();
        }
    }



    private void AddListenerToButtons()
    {
        // remove
        stage1Button.onClick.RemoveAllListeners();
        mainMenuButton.onClick.RemoveAllListeners();

        // register
        stage1Button.onClick.AddListener(OnClickStage1Button);
        mainMenuButton.onClick.AddListener(OnClickMainMenuButton);
    }

    private void OnClickStage1Button()
    {
        gameOverPopup.HideUI();

        Manager.Instance.sceneTransitionManager.SceneTransition("Stage1", true, false);
        Player.Instance.Revive(true);
    }

    private void OnClickMainMenuButton()
    {
        gameOverPopup.HideUI();

        Player.Instance.Revive(true);
        Manager.Instance.sceneTransitionManager.SceneTransition("MainMenu", true, true);
    }
}
