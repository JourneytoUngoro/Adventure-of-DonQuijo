using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    public bool isNull { get; set; } = true;
    public string profileId { get; set; }
    public string lastPlayTime { get; set; }
    public string playerHP { get; set; }
    public string memoryFragment { get; set; }

    [HideInInspector] public Button saveSlotButton;
    [HideInInspector] public Button deleteButton;
    [HideInInspector] public TextMeshProUGUI hpTMP;             // HP TMP 
    [HideInInspector] public TextMeshProUGUI mfTMP;             // MF TMP
    [HideInInspector] public TextMeshProUGUI playTimeTMP;      // PlayTime TMP 
    [HideInInspector] public TextMeshProUGUI emptySlotTMP;    // Empty TMP
    private Transform infoTmps;                                          // Slot Info Tmps
    [Tooltip("slot id : 0, 1, 2 ...")]
    [field: SerializeField] public int slotId { get; set; }

    LoadGameUI loadGameUI;

    private void Start()
    {
        loadGameUI = GetComponentInParent<LoadGameUI>();

        saveSlotButton = GetComponent<Button>();
        deleteButton = transform.Find("Delete Button").GetComponent<Button>();
        infoTmps = transform.Find("Slot Info Tmps").transform;

        hpTMP = infoTmps.Find("HP TMP").GetComponent<TextMeshProUGUI>();
        mfTMP = infoTmps.Find("MF TMP").GetComponent<TextMeshProUGUI>();
        playTimeTMP = infoTmps.Find("PlayTime TMP").GetComponent<TextMeshProUGUI>();
        emptySlotTMP = infoTmps.Find("Empty TMP").GetComponent<TextMeshProUGUI>();

        deleteButton.gameObject.SetActive(false);

        SetEvents();
    }

    void SetEvents()
    {
        saveSlotButton.onClick.RemoveAllListeners();
        deleteButton.onClick.RemoveAllListeners();

        saveSlotButton.onClick.AddListener(OnClickSaveSlotButton);
        deleteButton.onClick.AddListener(OnClickDeleteButton);
    }

    public void SetData(GameData data)
    {
        if (data != null)
        {
            isNull = false;
            playerHP = data.currentHealth.ToString();
            memoryFragment = "수정해";// data.questsData.questDatas[0].currentProgress.ToString(); 

            if (data.questsData.questCount > 0)
            {
                memoryFragment = data.questsData.GetQuestData("collectMemoryFragment").currentProgress.ToString();
            }
            else
            {
                memoryFragment = "-1"; // TODO : change to 0
            }
            lastPlayTime = data.displayedLastPlayTime;
        }
        else
        {
            isNull = true;
        }

        SetSlotView();
    }

    public void SetSlotView()
    {
        if (!isNull)
        {
            saveSlotButton.enabled = true;
            hpTMP.text = $"HP : {playerHP}";
            mfTMP.text = $"MF : {memoryFragment}";
            playTimeTMP.text = lastPlayTime;
            emptySlotTMP.text = string.Empty;
        }
        else
        {
            hpTMP.text = string.Empty;
            mfTMP.text = string.Empty;
            playTimeTMP.text = string.Empty;
            emptySlotTMP.text = "빈 슬롯";
        }
    }

    void OnClickDeleteButton()
    {
        Debug.Log("OnClickDeleteButtonSaveSlot");
        loadGameUI.OnClickDeleteButton(slotId);
    }

    void OnClickSaveSlotButton()
    {
        loadGameUI.OnClickSlotButton(slotId);
    }
}