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
            // 플레이어 살려
            bool success = Manager.Instance.itemManager.UseItem(index);
            Debug.Log("기억의 조각있어용");
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
        Debug.Log($"return to Stage1 : " + Manager.Instance.uiManager.activatedPopups.Count);
        Debug.Log($"return to Stage1 : " + Manager.Instance.inputHandler.IsCharacterControlEnabled());
        Debug.Log($"return to Stage1 : " + Manager.Instance.inputHandler.IsUIControlEnabled());


        Manager.Instance.sceneTransitionManager.SceneTransition("Stage1", true, true);
        Player.Instance.Revive();
    }

    private void OnClickMainMenuButton()
    {
        // 씬 전환
        // 정보 저장 -> 아니 근데 죽었는데 메인 가면 먼 정보를 저장해야 하니? 

        gameOverPopup.HideUI();
        Debug.Log($"return to Stage1 : " + Manager.Instance.uiManager.activatedPopups.Count);
        Debug.Log($"return to Stage1 : " + Manager.Instance.inputHandler.IsCharacterControlEnabled());
        Debug.Log($"return to Stage1 : " + Manager.Instance.inputHandler.IsUIControlEnabled());


        Player.Instance.transform.position = new Vector3(-600.0f, -100.0f, 0.0f);
        Manager.Instance.sceneTransitionManager.SceneTransition("MainMenu", true, true);
    }



}
