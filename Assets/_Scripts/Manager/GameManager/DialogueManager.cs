using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    #region 변수 선언
    [Title("Params")]
    [InfoBox("Dialogue Text Speed")][SerializeField] private float typingSpeed = 0.04f;

    [Title("Ink Globals JSON")]
    [SerializeField] private TextAsset globalsJSON;

    [Title("Main Character SO")]
    [SerializeField] private NPCDialogueSO mainCharacterSO;

    [TabGroup("Tab", "Dialogue UI")][SerializeField] private GameObject dialoguePanel;
    [TabGroup("Tab", "Dialogue UI")][SerializeField] private GameObject continueIcon;
    [TabGroup("Tab", "Dialogue UI")][SerializeField] private TextMeshProUGUI dialogueTMP;
    [TabGroup("Tab", "Dialogue UI")][SerializeField] private TextMeshProUGUI displayNameTMP;
    [TabGroup("Tab", "Dialogue UI")][SerializeField] private Image currentSpeakerImage;
    private NPCDialogueSO npcSO;
    private Animator layoutAnimator;

    [TabGroup("Tab", "Choices UI")][SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesTMP;

    [TabGroup("Tab", "Audio")][SerializeField] private AudioClip defaultAudioClip;
    private AudioClip playerAudioClip;
    private AudioClip currentAudioClip;

    private AudioSource audioSource;

    private Story currentStory;

    private string speaker;
    private bool canContinueToNextLine = false;
    private Coroutine displayLineCoroutine;

    public bool isDialoguePlaying { get; private set; }
    public static DialogueManager Instance { get; private set; } // TODO : Managers 통합

    private const string SpeakerTag = "speaker";
    private const string PortraitTag = "portrait";
    private const string AudioTag = "audio";

    private DialogueVariables dialogueVariables;
    private InkExternalFunction inkExternalFunction;

    #endregion 

    private void Awake()
    {
        #region Singleton
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        #endregion

        dialogueVariables = new DialogueVariables(globalsJSON);
        inkExternalFunction = new InkExternalFunction();

        audioSource = this.gameObject.AddComponent<AudioSource>();
        currentAudioClip = defaultAudioClip;
    }

    private void Start()
    {
        isDialoguePlaying = false;
        dialoguePanel.SetActive(false);

        layoutAnimator = dialoguePanel.GetComponent<Animator>();

        choicesTMP = new TextMeshProUGUI[choices.Length];

        playerAudioClip = mainCharacterSO.npcAudioClip;

        int index = 0;
        foreach (var choice in choices)
        {
            choicesTMP[index++] = choice.GetComponentInChildren<TextMeshProUGUI>();
        }

    }

    private bool PressedNextLineKey()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    private void SetCurrentAudioClip(AudioClip clip)
    {
        // set speaker's sound
        AudioClip audioInfo = null;

        if (audioInfo != null)
        {
            this.currentAudioClip = audioInfo;
        }
        else
        {
            Debug.LogWarning($"Failed to find audio info {clip.name}");
        }
    }

    private void Update()
    {
        if (!isDialoguePlaying) return;

        if (canContinueToNextLine
            && currentStory.currentChoices.Count == 0
            && PressedNextLineKey())
        {
            ContinueStory();
        }
    }

    public void EnterDialogue(TextAsset inkJSON, Animator anim, NPCDialogueSO npcSO)
    {
        Debug.Log("updated version executing...");

        currentStory = new Story(inkJSON.text);
        this.npcSO = npcSO;

        isDialoguePlaying = true;
        dialoguePanel.SetActive(true);

        dialogueVariables.StartListening(currentStory);
        inkExternalFunction.Bind(currentStory, anim);

        ContinueStory();
    }

    private IEnumerator ExitDialogue()
    {
        // prevent duplicate key input over multiple frames
        yield return new WaitForSeconds(0.2f);

        dialogueVariables.StopListening(currentStory);
        inkExternalFunction.Unbind(currentStory);

        this.npcSO = null;

        isDialoguePlaying = false;
        dialoguePanel.SetActive(false);
        dialogueTMP.text = "";

        SetCurrentAudioClip(defaultAudioClip);
    }

    private void ContinueStory()
    {
        // no more prepared dialogues
        if (!currentStory.canContinue)
        {
            StartCoroutine(ExitDialogue());
            return;
        }

        if (displayLineCoroutine != null)
        {
            StopCoroutine(displayLineCoroutine);
        }
        string nextLine = currentStory.Continue();

        if (nextLine.Equals("") && !currentStory.canContinue)
        {
            StartCoroutine(ExitDialogue());
        }
        else
        {
            HandleTags(currentStory.currentTags);
            PlayDialogueSound();
            displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        dialogueTMP.text = line;
        dialogueTMP.maxVisibleCharacters = 0;

        continueIcon.SetActive(false);
        HideChoices();

        canContinueToNextLine = false;
        bool hasRichText = false;

        foreach (char c in line.ToCharArray())
        {
/*            if (PressedNextLineKey())
            {
                Debug.Log("skip display chars...");
                dialogueTMP.maxVisibleCharacters = line.Length;
                break;
            }*/

            if (c == '<' || hasRichText)
            {
                hasRichText = true;
                if (c == '>')
                {
                    hasRichText = false;
                }
            }
            else
            {
                dialogueTMP.maxVisibleCharacters++;

                yield return new WaitForSeconds(typingSpeed);
            }
        }

        continueIcon.SetActive(true);
        DisplayChoices();

        canContinueToNextLine = true;
        if (audioSource.isPlaying) audioSource.Stop();
    }

    private void PlayDialogueSound()
    {
        audioSource.Stop();

        // play
        audioSource.PlayOneShot(currentAudioClip);
    }

    private void HideChoices()
    {
        foreach (var choiceButton in choices)
        {
            choiceButton.gameObject.SetActive(false);
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            Debug.Assert(splitTag.Length == 2, "tag doesn't be appropriately parsed " + tag);

            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey)
            {
                case SpeakerTag:
                    speaker = tagValue;
                    displayNameTMP.text = speaker;
                    break;
                case PortraitTag:
                    CurrentPortrait(speaker, tagValue);
                    break;
                case AudioTag:
                    break;
                default:
                    Debug.LogWarning("tag is not currently being handled " + tag);
                    break;
            }
        }

        CurrentAudioClip(speaker);
        CurrentLayout(speaker);

        speaker = string.Empty;
    }

    private void CurrentAudioClip(string speaker)
    {
        // 출력음이 많아진다면 Dictionary 로 변경해서 audio clip을 관리해야 한다 
        /*        const string MainCharacter = "DonQuijo";
                const string NPC = "NPC";*/

        const string MainCharacter = "Jiyon";

        if (speaker == MainCharacter)
        {
            currentAudioClip = playerAudioClip;
        }
        else
        {
            currentAudioClip = npcSO.npcAudioClip;

        }
    }

    private void CurrentPortrait(string speaker, string tagValue)
    {
        /*        const string MainCharacter = "DonQuijo";
                const string NPC = "NPC";*/

        const string MainCharacter = "Jiyon";

        if (speaker == MainCharacter)
        {
            mainCharacterSO.ApplyPortraitSprite(currentSpeakerImage, tagValue);
        }
        else
        {
            npcSO.ApplyPortraitSprite(currentSpeakerImage, tagValue);

        }
    }

    private void CurrentLayout(string speaker)
    {
        /*        const string MainCharacter = "DonQuijo";
                const string NPC = "NPC";*/

        const string MainCharacter = "Jiyon";

        if (speaker == MainCharacter)
        {
            layoutAnimator.Play("left");
        }
        else
        {
            layoutAnimator.Play("right");

        }
    }



    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;
        Debug.Assert(currentChoices.Count <= choices.Length,
            $"More choiceButton button needed : {currentChoices.Count} > {choices.Length}");

        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesTMP[index].text = choice.text;
            index++;
        }

        // Inactive buttons exceeding the number of current choices
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        if (canContinueToNextLine)
        {
            currentStory.ChooseChoiceIndex(choiceIndex);
            ContinueStory();
        }
    }

    public Ink.Runtime.Object GetVariableState(string variableName)
    {
        Ink.Runtime.Object variableValue = null;
        dialogueVariables.variables.TryGetValue(variableName, out variableValue);

        Debug.Assert(variableValue != null, "Ink variable is null " + variableName);

        return variableValue;
    }

    public void OnApplicationQuit()
    {
        dialogueVariables.SaveVariables();
    }
}
