using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isNull { get; set; } = true;
    public string profileId { get; set;  }
    public string lastPlayTime { get; set; }
    public string playerHP { get; set; }
    public string memoryFragment { get; set; }

    [HideInInspector] public Button saveSlotButton;
    [HideInInspector] public Button deleteButton;

    [Tooltip("slot id : 0, 1, 2 ...")]
    [field: SerializeField] public int slotId { get; set; }

    LoadGameUI loadGameUI;
    TextInfoUI textInfo;

    private void Start()
    {
        loadGameUI = GetComponentInParent<LoadGameUI>();

        saveSlotButton = GetComponent<Button>();
        deleteButton = transform.Find("Delete Button").GetComponent<Button>();
        textInfo = GetComponentInChildren<TextInfoUI>();

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
            textInfo.SetDynamicTextInfo(new TextInfoData(
                $"수정하시오 "));
        }
        else
        {
            // textInfo.SetDynamicTextInfo(new TextInfoData($"              데이터 없음            \n "));
            saveSlotButton.transform.Find("Slot TMP").GetComponent<TextMeshProUGUI>().text = "빈 슬롯";
        }
    }

    void OnClickDeleteButton()
    {
        loadGameUI.OnClickDeleteButton(slotId);
    }

    void OnClickSaveSlotButton()
    {
        loadGameUI.OnClickSlotButton(slotId);
    }

    public void OnHorverEnter()
    {
        textInfo.ShowUI();
    }

    public void OnHorverExit()
    {
        textInfo.HideUI();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!isNull)
            OnHorverEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isNull)
            OnHorverExit();
    }

/*    public void SetOutline(bool visibility)
    {
        outline.enabled = visibility;
    }*/
}
