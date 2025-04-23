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
    public float mentality {  get; set; }
    public string lastPlayTime { get; set; }
    public float totalPlayTime { get; set; }
    public string stage { get; set; }

    [HideInInspector] public Button saveSlotButton;
    [HideInInspector] public Button deleteButton;

    [Tooltip("slot id : 0, 1, 2 ...")]
    [field: SerializeField] public int slotId { get; set; }

    LoadGameUI loadGameUI;
    TextInfoUI textInfo;
    Outline outline;

    private void Start()
    {
        loadGameUI = GetComponentInParent<LoadGameUI>();

        saveSlotButton = GetComponent<Button>();
        deleteButton = transform.Find("Delete Button").GetComponent<Button>();
        textInfo = GetComponentInChildren<TextInfoUI>();
        outline = GetComponent<Outline>();

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

            mentality = data.mentality;
            lastPlayTime = data.displayedLastPlayTime;
            totalPlayTime = data.totalPlayTime;
            stage = data.currentScene;
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
            textInfo.SetDynamicTextInfo(new TextInfoData(
                $"Mentality : {mentality}\nLast Play Time : {lastPlayTime}\nTotal Play Time : {totalPlayTime}\nstage : {stage}"));
        }
        else
        {
            textInfo.SetDynamicTextInfo(new TextInfoData($"No Data"));
            saveSlotButton.transform.Find("Slot TMP").GetComponent<TextMeshProUGUI>().text = "empty\nslot";
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
        OnHorverEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHorverExit();
    }

    public void SetOutline(bool visibility)
    {
        outline.enabled = visibility;
    }
}
