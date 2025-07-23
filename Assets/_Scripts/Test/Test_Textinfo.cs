using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Textinfo : MonoBehaviour
{
    public UIManager UIManager;
    public void ShowAndHide()
    {
        UIManager.ShowDynamicTextInfo(
            new TextInfoData("저장에 성공했습니다.")).ShowAndHideUI(2f);
    }

}
