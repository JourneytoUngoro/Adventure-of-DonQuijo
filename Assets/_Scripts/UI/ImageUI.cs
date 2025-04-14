using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageUI : UIBase
{
    // 오브젝트                      // 씬 상의 오브젝트 이름
    Image backgroundImage; // Background Image

    public override void ShowUI()
    {
        base.ShowUI();

        UIManager.Instance.activatedImages.Push(this);
    }

    public override void HideUI()
    {
        base.HideUI();

        UIManager.Instance.activatedImages.TryPop(out _);
    }

    public override void ShowAndHideUI(float waitTime)
    {
        base.ShowAndHideUI(waitTime);
    }

    public ImageUI SetDynamicImage(Image image)
    {
        backgroundImage = image;

        return this;
    }

    public override void Move(Vector2 direction, bool ease)
    {
        base.Move(direction, ease);
        // TODO : 패널 움직이는 기능 구현 
    }
    protected override void ReturnToPool()
    {
        UIManager.Instance.imagePool.Return(this);
    }

}
