using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameClearController : MonoBehaviour
{
    public Button mainMenuButton;
    public TextMeshProUGUI displayDataTMP;

    private PopupUI clearPopupUI;

    public void TEST_DisplayTMP()
    {
        clearPopupUI.ShowUI();
        ShowPlayInfo();
    }


    private void Awake()
    {
        Initialize();
        AddListenerToUI();
    }

    public void Initialize()
    {
        clearPopupUI = GetComponent<PopupUI>();
    }

    public void ShowPlayInfo()
    {
        Manager.Instance.dataManager.SaveGame();
        GameData clearedStageData = Manager.Instance.dataManager.gameData;

        string playTime = $"{(int)(clearedStageData.totalPlayTime / 60f)} 분 {(int)(clearedStageData.totalPlayTime % 60)} 초";
        string clearedTime = clearedStageData.displayedLastPlayTime;
        string killedEnemy = clearedStageData.stageData.killedEnemtyCount.ToString();
        string collectedMemory = clearedStageData.stageData.collectedFragmentCount.ToString();

        string[] lines =
        {
            $"총 플레이 타임 : {playTime}\n",
            $"클리어 시간 : {clearedTime}\n",
            $"수집한 메모리 조각 : {collectedMemory} 개\n",
            $"처치한 적 : {killedEnemy} 마리\n"
         };

        displayDataTMP.text = ""; 

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(clearPopupUI.FadeTime());

        float delayPerLine = 0.3f;

        for (int i = 0; i < lines.Length; i++)
        {
            int index = i;
            seq.AppendInterval(delayPerLine);
            seq.AppendCallback(() =>
            {
                displayDataTMP.text += lines[index] + "\n";
            });
        }
    }




    private void AddListenerToUI()
    {
        mainMenuButton.onClick.RemoveAllListeners();

        mainMenuButton.onClick.AddListener(OnClickMainMenuButton);
    }

    public void OnClickMainMenuButton()
    {
        clearPopupUI.HideUI();
        Manager.Instance.sceneTransitionManager.SceneTransition("MainMenu", true, true);
        Destroy(gameObject);
    }

}
