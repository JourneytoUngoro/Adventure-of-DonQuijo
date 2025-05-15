using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityData : ScriptableObject
{
    [field: Header("Entity Data")]
    [field: SerializeField] public float entityStature { get; private set; } = 80.0f;
    [field: SerializeField] public float gravityScale { get; private set; } = 100.0f;

    [field: Header("Move State")]
    [field: SerializeField] public Vector2 moveSpeed { get; private set; } = new Vector2(150, 75);
    [field: SerializeField] public Vector2 dashSpeed { get; private set; } = new Vector2(300, 150);

    [field: Header("Jump State")]
    [field: SerializeField] public float jumpSpeed { get; private set; } = 500.0f;

    [field: Header("In Air State")]
    [field: SerializeField] public float gotoLandingStateTime { get; private set; } = 3.0f;

    [field: Header("Stunned State")]
    [field: SerializeField] public float stunRecoveryTime { get; private set; } = 3.0f;

    [field: Header("Landing State")]
    [field: SerializeField] public float landingRecoveryTime { get; private set; } = 1.0f;

    [field: Header("Knockback State")]
    [field: SerializeField] public float knockbackReboundThresholdSpeed { get; private set; } = 300.0f;
    [field: SerializeField] public float wallKnockbackReboundThresholdAngle { get; private set; } = 30.0f;
    [field: SerializeField] public float decelerationRatio { get; private set; } = 0.7f;
}
