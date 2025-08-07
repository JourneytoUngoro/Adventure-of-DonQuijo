using TMPro;
using UnityEngine;
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
    [HideInInspector] public Image mfImage;                           // MF Image
    [HideInInspector] public TextMeshProUGUI playTimeTMP;      // PlayTime TMP 
    [HideInInspector] public TextMeshProUGUI emptySlotTMP;    // Empty TMP
    private Transform infoTmps;                                          // Slot Info Tmps
    [HideInInspector] public ButtonScaler buttonsScaler;

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
        mfImage = mfTMP.transform.Find("MF Image").GetComponent<Image>();
        playTimeTMP = infoTmps.Find("PlayTime TMP").GetComponent<TextMeshProUGUI>();
        emptySlotTMP = infoTmps.Find("Empty TMP").GetComponent<TextMeshProUGUI>();

        buttonsScaler = GetComponent<ButtonScaler>();

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
            lastPlayTime = data.displayedLastPlayTime;

            if (data.stageData.collectedFragmentCount > 0) { memoryFragment = data.stageData.collectedFragmentCount.ToString(); }
            else { memoryFragment = "0"; }
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
            mfTMP.text = $"x {memoryFragment}";
            mfImage.gameObject.SetActive(true);
            playTimeTMP.text = lastPlayTime;
            emptySlotTMP.text = string.Empty;
        }
        else
        {
            hpTMP.text = string.Empty;
            mfTMP.text = string.Empty;
            mfImage.gameObject.SetActive(false);
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
        if (loadGameUI.editing)
        {
            loadGameUI.OnClickDeleteButton(slotId);
        }
        else
        {
            loadGameUI.OnClickSlotButton(slotId);
        }
    }
}