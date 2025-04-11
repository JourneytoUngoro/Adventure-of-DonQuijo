using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : CombatAbilityComponent
{
    [field: SerializeField] public Vector2 planeDirection { get; private set; }
    [field: SerializeField] public float planeVelocity { get; private set; }
    [field: SerializeField] public float moveTime { get; private set; }
    [field: SerializeField] public Ease easeFunction { get; private set; }
    [field: SerializeField] public bool slowDown { get; private set; }
    [field: SerializeField] public float orthogonalVelocity { get; private set; }

    public override void ApplyCombatAbility(Entity target, OverlapCollider[] overlapColliders)
    {
        throw new System.NotImplementedException();
    }
}
