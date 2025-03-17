using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleData : MonoBehaviour
{
    [field: SerializeField] public float obstacleHeight { get; private set; }
    [field: SerializeField] public bool moveable { get; private set; }
}
