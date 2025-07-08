using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "QuestInfo", menuName = "Scriptable Object/QuestInfoSO")]
public class QuestInfoSO : ScriptableObject
{
    public string id;
    public string questTitle;
    public string questDescription;

    public int goalProgress;
    public GameObject reward; 
    
    public bool isAutoStart = true;
    public bool isAutoComplete = true; 
}
