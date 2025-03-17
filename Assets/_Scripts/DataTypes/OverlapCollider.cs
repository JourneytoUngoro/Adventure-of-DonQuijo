using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct OverlapCollider
{
    // [HideInInspector] public string name;
    [field: SerializeField] public Transform centerTransform { get; private set; }
    [field: SerializeField] public bool overlapBox { get; private set; }
    [field: SerializeField] public bool overlapCircle { get; private set; }

    [field: SerializeField] public Vector2 boxSize { get; private set; }
    [field: SerializeField, Range(-180.0f, 180.0f), Tooltip("Positive value rotates to counter clock wise direction.")] public float boxRotation { get; private set; }
    [field: SerializeField] public float circleRadius { get; private set; }

    [field: SerializeField] public bool limitAngle { get; private set; }
    [field: SerializeField, Range(-180.0f, 180.0f), Tooltip("Positive value rotates to counter clock wise direction.")] public float centerRotation { get; private set; }
    [field: SerializeField, Range(0.0f, 180.0f)] public float clockwiseAngle { get; private set; }
    [field: SerializeField, Range(0.0f, 180.0f)] public float counterClockwiseAngle { get; private set; }
}
