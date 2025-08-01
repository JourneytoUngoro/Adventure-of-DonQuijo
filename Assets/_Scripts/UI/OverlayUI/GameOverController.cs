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
            Player.Instance.Revive();
        }
        else
        {
            // 아니면 이제 그냥 있는거 버튼 선택하게 해.
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
        // 캐릭터 초기화
        // 씬 전환
        // 해준님 함수 호출

        gameOverPopup.HideUI();

        Player.Instance.Revive();
        Manager.Instance.sceneTransitionManager.SceneTransition("Stage1", true, false);
        Manager.Instance.inputHandler.SetCharacterControlEnabled(true);
    }

    private void OnClickMainMenuButton()
    {
        // 씬 전환
        // 정보 저장 -> 아니 근데 죽었는데 메인 가면 먼 정보를 저장해야 하니? 

        gameOverPopup.HideUI();

        Player.Instance.Revive();
        Manager.Instance.sceneTransitionManager.SceneTransition("MainMenu", true, true);
    }



}
