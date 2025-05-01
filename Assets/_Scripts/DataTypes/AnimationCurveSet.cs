using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationCurveSet
{
    [field: SerializeField] public AnimationCurve incrementPerLevel { get; private set; }
    [field: SerializeField] public AnimationCurve accumulationPerLevel { get; private set; }
}
