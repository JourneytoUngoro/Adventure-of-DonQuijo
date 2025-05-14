using TMPro;
using UnityEngine;

public class AutoSizeTextInfo : MonoBehaviour
{
    public RectTransform background; // 배경 이미지
    public TextMeshProUGUI tmpText;

    public Vector2 padding = new Vector2(20f, 20f); // 여백

    void LateUpdate()
    {
        Vector2 textSize = tmpText.GetPreferredValues(tmpText.text);
        background.sizeDelta = textSize + padding;
    }
}