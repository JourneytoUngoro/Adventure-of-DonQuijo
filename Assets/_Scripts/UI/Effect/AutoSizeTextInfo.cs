using TMPro;
using UnityEngine;

public class AutoSizeTextInfo : MonoBehaviour
{
    public RectTransform background; 
    public TextMeshProUGUI tmpText;

    public Vector2 padding = new Vector2(20f, 20f); 

    void LateUpdate()
    {
        Vector2 textSize = tmpText.GetPreferredValues(tmpText.text);
        background.sizeDelta = textSize + padding;
    }
}