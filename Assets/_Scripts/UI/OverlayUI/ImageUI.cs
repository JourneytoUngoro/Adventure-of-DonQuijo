using System;
using UnityEngine;
using UnityEngine.UI;

public class ImageUI : UIBase
{
    // 오브젝트                      // 씬 상의 오브젝트 이름
    Image backgroundImage; // Background Image

    public ImageUI SetDynamicImage(Image image)
    {
        backgroundImage = image;

        return this;
    }

    protected override void AllowmentComponent()
    {
        
    }

    protected override void ReturnToPool()
    {
        if (type == UIType.DynamicImage)
        {
            Manager.Instance.uiManager.imagePool.Return(this);
        }
    }
}
