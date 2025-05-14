using System.Collections;
using System.Collections.Generic;
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
    private TextMeshProUGUI endingTMP;


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
        endingTMP = endingPanel.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ShowEndingPanel(bool win)
    {
        if (win)
        {
            endingTMP.text = "승리";
        }
        else
        {
            endingTMP.text = "게임 오버";
        }

        endingPanel.SetActive(true);
    }

    public void OnClickRetryBtn()
    {
        SceneManager.LoadScene(thisScene);
    }


}
