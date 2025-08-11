using UnityEngine;
using UnityEngine.UI;

public class ImageCropper : MonoBehaviour
{
    public RectTransform imageContainer;
    public Image image;


    void Awake()
    {
        // ScaleImage();
    }

    public void ScaleImage()
    {
        float containerWidth = imageContainer.rect.width;
        float containerHeight = imageContainer.rect.height;

        float spriteWidth = image.sprite.rect.width;
        float spriteHeight = image.sprite.rect.height;

        float containerAspect = (float)(containerWidth / containerHeight);
        float spriteAspect = (float)(spriteWidth / spriteHeight);

        if (containerAspect > spriteAspect)
        {
            float newWidth = containerWidth;
            float newHeight = (float)(newWidth / spriteWidth * spriteHeight);

            image.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
            image.rectTransform.anchoredPosition = new Vector2(0, (newHeight - containerHeight) / 2);
        }
        else
        {
            float newHeight = containerHeight;
            float newWidth = (float)(newHeight / spriteHeight * spriteWidth);

            image.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
            image.rectTransform.anchoredPosition = new Vector2((newHeight - containerHeight) / 2, 0);
        }
    }
}