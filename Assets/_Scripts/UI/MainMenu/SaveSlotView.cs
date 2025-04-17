using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveSlotView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [field: SerializeField] public string profileId { get; set;  }
    [field: SerializeField] public float mentality {  get; set; }
    [field: SerializeField] public string lastPlayTime { get; set; }
    [field: SerializeField] public float totalPlayTime { get; set; }
    [field: SerializeField] public string stage { get; set; }
    [field: SerializeField] public bool isNull { get; set; }

    [SerializeField] TextInfoUI textInfo;
    [SerializeField] GameObject outline;

    public void SetData(string profileId, GameData data)
    {
        outline.SetActive(false);

        if (profileId != null && data != null)
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
        }
        Debug.Log("Set Data");
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

    public void SetOutline(bool active)
    {
        outline.SetActive(active);
    }


}
