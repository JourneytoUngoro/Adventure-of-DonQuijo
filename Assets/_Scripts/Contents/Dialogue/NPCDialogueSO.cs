using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Dialogue/NPC Portrait")]
public class NPCDialogueSO : ScriptableObject
{
    public string npcName;
    public AudioClip npcAudioClip;

    [SerializeField] private List<PortraitEntry> portraitEntries;

    private Dictionary<string, Sprite> portraitDict;

    [System.Serializable]
    public class PortraitEntry
    {
        public string key;
        public Sprite sprite;
    }

    private void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        portraitDict = new Dictionary<string, Sprite>();
        foreach (var entry in portraitEntries)
        {
            if (!portraitDict.ContainsKey(entry.key))
            {
                portraitDict.Add(entry.key, entry.sprite);
            }
        }
    }

    private Sprite GetPortrait(string key)
    {
        if (portraitDict == null) Initialize();
        return portraitDict.TryGetValue(key, out var sprite) ? sprite : null;
    }

    public void ApplyPortraitSprite(Image image, string portraitKey)
    {
        Sprite sprite = GetPortrait(portraitKey);

        Debug.Assert(image != null, "SpriteRenderer is null!");
        Debug.Assert(sprite != null, "Sprite is null!");

        if (image != null && sprite != null)
        {
            image.sprite = sprite;
        }
    }

}
