using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "QuestInfo", menuName = "Scriptable Object/QuestInfoSO")]
public class QuestInfoSO : ScriptableObject
{
    [SerializeField] public string id { get; private set; }
    public string questTitle;
    public string questDescription;
    public int coinReward;
}
