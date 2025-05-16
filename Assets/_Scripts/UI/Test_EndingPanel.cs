using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 완전 완전 임시 함수 
/// </summary>
public class Test_EndingPanel : MonoBehaviour
{
    public static Test_EndingPanel instance;
    public SceneField thisScene;

    [SerializeField] private GameObject endingPanel;
    [SerializeField] private TextMeshProUGUI endingTMP;
    private Player player;


    private void Awake()
    {
        #region singleton
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        #endregion
    }

    private void Start()
    {
        endingPanel.SetActive(false);

        player = Manager.Instance.player;
        Debug.Log(player != null ? "ending panel found player" : "ending panel can't find player");
        Debug.Assert(player.deadState != null, "player deadState is null!");
        Debug.Assert(player.deadState.waitTimer != null, "player.deadState.waitTimeris null!");
        player.deadState.waitTimer.timerAction += ShowGameOverPanel;


    }

    public void ShowGameOverPanel()
    {
        Debug.Log("played show Game Over Panel");
        endingTMP.text = "게임 오버";
        endingPanel.SetActive(true);
    }

    public void ShowWinPanel()
    {
        endingTMP.text = "승리";
        endingPanel.SetActive(true);
    }

    public void OnClickRetryBtn()
    {
        SceneManager.LoadScene(thisScene);
    }


}
