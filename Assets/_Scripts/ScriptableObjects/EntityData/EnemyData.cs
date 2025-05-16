using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEnemyData", menuName = "Data/Enemy Data")]
public class EnemyData : EntityData
{
    [field: Header("Dazed State")]
    [field: SerializeField] public float dazedTime { get; private set; } = 2.0f;

    [field: Header("Target In Detection Range State")]
    [field: SerializeField] public float stepSize { get; private set; } = 5.0f;
    [field: SerializeField] public float maxDistance { get; private set; } = 80.0f;
    [field: SerializeField] public float adequateDistance { get; private set; } = 60.0f;
    [field: SerializeField] public float walkAroundDistance { get; private set; } = 20.0f;
    [field: SerializeField] public float repositioningPossibility { get; private set; } = 0.6f;
    [field: SerializeField] public float repositionOffsetDistance { get; private set; } = 10.0f;
    [field: SerializeField] public float repositioningTime { get; private set; } = 8.0f;

    [field: Header("Block / Parry State")]
    [field: SerializeField] public int maxBlockableCount { get; private set; } = 2;
    [field: SerializeField] public float blockCoolDownTime { get; private set; } = 3.0f;
    [field: SerializeField] public float blockPossibility { get; private set; } = 0.7f;
    [field: SerializeField] public float parryGaugeWhenBlocked { get; private set; } = 0.6f;
    [field: SerializeField] public int maxParryableCount { get; private set; } = 1;
    [field: SerializeField] public float blockParryTime { get; private set; } = 0.4f;

    [field: Header("MeleeAttack State")]
    [field: SerializeField] public float meleeAttack0CoolDown { get; private set; } = 4.0f;
    [field: SerializeField] public float meleeAttack1CoolDown { get; private set; } = 10.0f;
    [field: SerializeField] public float meleeAttack2CoolDown { get; private set; } = 4.0f;
    [field: SerializeField] public float dashAttackCoolDown { get; private set; } = 15.0f;
    [field: SerializeField] public float dodgeAttackCoolDown { get; private set; } = 15.0f;
    [field: SerializeField] public float wideAttackCoolDown { get; private set; } = 10.0f;

    [field: Header("Canvas Settings")]
    [field: SerializeField] public float canvasDisableTime { get; private set; } = 5.0f;
}
