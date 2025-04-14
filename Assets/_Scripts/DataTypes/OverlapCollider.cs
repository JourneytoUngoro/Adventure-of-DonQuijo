using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct OverlapCollider
{
    [field: SerializeField] public Collider2D overlapCollider { get; private set; }
    [field: SerializeField] public float height { get; private set; }
    [field: SerializeField, Tooltip("Limiting angle is not affected by rotation of the collider's rotation.")] public bool limitAngle { get; private set; }
    [field: SerializeField, Range(0.0f, 180.0f)] public float clockwiseAngle { get; private set; }
    [field: SerializeField, Range(0.0f, 180.0f)] public float counterClockwiseAngle { get; private set; }
}
