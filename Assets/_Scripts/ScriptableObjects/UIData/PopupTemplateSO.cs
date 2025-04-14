using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/PopupTemplate")]
public class PopupTemplateSO : ScriptableObject
{
    public string title;
    public string info;
    public string confirmText;
    public string cancelText;
}
