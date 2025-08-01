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
        // In case, player has memory fragment(another life)
        // 이건 제가 추가하겠습니닥 
        // 여기다가 구현 안 할지도 

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

        Manager.Instance.sceneTransitionManager.SceneTransition("Stage1", true, true);
        Player.Instance.Revive();
    }

    private void OnClickMainMenuButton()
    {
        // 씬 전환
        // 정보 저장 -> 아니 근데 죽었는데 메인 가면 먼 정보를 저장해야 하니? 

        gameOverPopup.HideUI();



    }




}
