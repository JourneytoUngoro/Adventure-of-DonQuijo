using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileComponent : CombatAbilityComponent
{
    [field: SerializeField] public GameObject projectilePrefab { get; private set; }
    [field: SerializeField] public LayerMask whatIsDamageable { get; private set; }
    [field: SerializeField] public float projectileSpeed { get; private set; }

    public override void ApplyCombatAbility(Collider2D target, OverlapCollider[] overlapColliders)
    {
        throw new System.NotImplementedException();
    }
}
