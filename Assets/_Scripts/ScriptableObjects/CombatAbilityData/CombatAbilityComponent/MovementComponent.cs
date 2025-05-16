using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DashDirection { Facing, Absolute, TowardTarget }

public class MovementComponent : CombatAbilityComponent
{
    [field: SerializeField] public bool isDashAttack { get; private set; }
    [field: SerializeField] public DashDirection dashDirection { get; private set; }
    [field: SerializeField] public Vector2 planeDirection { get; private set; }
    [field: SerializeField] public float planeVelocity { get; private set; }
    [field: SerializeField, Min(0.0f)] public float moveTime { get; private set; }
    [field: SerializeField, Min(0.0f)] public float attackStartTime { get; private set; }
    [field: SerializeField, Min(0.0f)] public float attackFinishTime { get; private set; }
    [field: SerializeField] public Ease easeFunction { get; private set; }
    [field: SerializeField] public AnimationCurve easeCurve { get; private set; }
    [field: SerializeField] public bool reverseTime { get; private set; }
    [field: SerializeField] public float orthogonalVelocity { get; private set; }

    public override void ApplyCombatAbility(Collider2D target, OverlapCollider[] overlapColliders)
    {
        pertainedCombatAbility.sourceEntity.entityCombat.GetMovement(this, overlapColliders);
    }
}
