using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DirectionBase { Position, Absolute, sourceEntityFacingDirection, positionRelativeDirection }

public class KnockbackComponent : CombatAbilityComponent
{
    [field: SerializeField] public bool airborne { get; private set; } = false;
    [field: SerializeField] public DirectionBase directionBase { get; private set; } = DirectionBase.Position;
    [field: SerializeField] public Vector2 knockbackDirection { get; private set; }
    [field: SerializeField] public float knockbackSpeed { get; private set; }
    [field: SerializeField, Tooltip("KnockbackTime of 0 means that the entity will transit from knockbackState when it hits the ground.")] public float knockbackTime { get; private set; }
    [field: SerializeField] public Ease easeFunction { get; private set; }
    [field: SerializeField] public float orthogonalVelocity { get; private set; }

    [field: SerializeField] public Vector2 knockbackDirectionWhenBlocked { get; private set; }
    [field: SerializeField] public float knockbackSpeedWhenBlocked { get; private set; }
    [field: SerializeField] public float knockbackTimeWhenBlocked { get; private set; }
    [field: SerializeField] public Ease easeFunctionWhenBlocked { get; private set; }

    [field: SerializeField] public Vector2 knockbackDirectionWhenParried { get; private set; }
    [field: SerializeField] public float knockbackSpeedWhenParried { get; private set; }
    [field: SerializeField] public float knockbackTimeWhenParried { get; private set; }
    [field: SerializeField] public Ease easeFunctionWhenParried { get; private set; }

    [field: SerializeField] public bool counterProtrudedWhenParried { get; private set; }
    [field: SerializeField] public Vector2 counterKnockbackDirectionWhenParried { get; private set; }
    [field: SerializeField] public float counterKnockbackSpeedWhenParried { get; private set; }
    [field: SerializeField] public float counterKnockbackTimeWhenParried { get; private set; }
    [field: SerializeField] public Ease counterEaseFunctionWhenParried { get; private set; }
    [field: SerializeField] public float counterOrthogonalVelocityWhenParried { get; private set; }
    public Transform knockbackSourceTransform { get; set; }

    public override void ApplyCombatAbility(Collider2D target, OverlapCollider[] overlapColliders)
    {
        target.GetComponentInParent<Entity>().entityCombat.GetKnockback(this, overlapColliders);
    }
}
