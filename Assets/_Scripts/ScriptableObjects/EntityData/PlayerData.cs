using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data")]
public class PlayerData : EntityData
{
    [field: Header("Control Data")]
    [field: SerializeField] public float dashInputBufferTime { get; private set; } = 0.5f;
    [field: SerializeField] public float dashMaintainTime { get; private set; } = 0.1f;
    [field: SerializeField] public float jumpInputBufferTime { get; private set; } = 0.2f;
    [field: SerializeField] public float attackInputBufferTime { get; private set; } = 0.2f;
    [field: SerializeField] public float variableJumpHeightMultiplier { get; private set; } = 0.5f;
    [field: SerializeField] public float coyoteTime { get; private set; } = 0.2f;

    [field: Header("Wall Jump State")]
    [field: SerializeField] public float wallJumpSpeed { get; private set; } = 60.0f;
    [field: SerializeField] public float wallTouchHeight { get; private set; } = 11.5f;

    [field: Header("Dodge State")]
    [field: SerializeField] public float dodgeHeight { get; private set; } = 40.0f;
    [field: SerializeField] public float dodgeSpeed { get; private set; } = 80.0f;
    [field: SerializeField] public float backstepSpeed { get; private set; } = 60.0f;
    [field: SerializeField] public float dodgeCoolDownTime { get; private set; } = 1.0f;
    [field: SerializeField] public float dodgeTime { get; private set; } = 0.4f;
    [field: SerializeField] public float backstepTime { get; private set; } = 0.2f;

    [field: Header("ShieldParry State")]
    [field: SerializeField] public float shieldParryCoolDownTime { get; private set; } = 1.0f;

    [field: Header("Attack State")]
    [field: SerializeField] public float attackStrokeTime { get; private set; } = 2.0f;
}