using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : EntityData
{
    [field: Header("Dazed State")]
    [field: SerializeField] public float dazedTime = 2.0f;

    [field: Header("Canvas Settings")]
    [field: SerializeField] public float canvasDisableTime = 5.0f;
}
