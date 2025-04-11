using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEnemyData", menuName = "Data/Enemy Data")]
public class EnemyData : EntityData
{
    [field: Header("Dazed State")]
    [field: SerializeField] public float dazedTime = 2.0f;

    [field: Header("Canvas Settings")]
    [field: SerializeField] public float canvasDisableTime = 5.0f;
}
