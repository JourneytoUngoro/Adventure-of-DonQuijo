using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityData : ScriptableObject
{
    [field: Header("Entity Data")]
    [field: SerializeField] public float entityHeight { get; private set; } = 16.0f;
    [field: SerializeField] public float gravityScale { get; private set; } = 20.0f;

    [field: Header("Move State")]
    [field: SerializeField] public Vector2 moveSpeed { get; private set; } = new Vector2(30, 15);
    [field: SerializeField] public Vector2 dashSpeed { get; private set; } = new Vector2(60, 30);

    [field: Header("In Air State")]
    [field: SerializeField] public float gotoLandingStateTime { get; private set; } = 3.0f;

    [field: Header("Stunned State")]
    [field: SerializeField] public float stunRecoveryTime { get; private set; } = 3.0f;

    [field: Header("Landing State")]
    [field: SerializeField] public float landingRecoveryTime { get; private set; } = 1.0f;
}
