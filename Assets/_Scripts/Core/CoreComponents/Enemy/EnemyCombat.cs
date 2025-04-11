using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : Combat
{
    protected Enemy enemy;
    [field: SerializeField] public List<CombatAbilityWithColliders> meleeAttack0 { get; private set; }
    [field: SerializeField] public List<CombatAbilityWithColliders> meleeAttack1 { get; private set; }
    [field: SerializeField] public List<CombatAbilityWithColliders> meleeAttack2 { get; private set; }
    [field: SerializeField] public List<CombatAbilityWithColliders> meleeAttack3 { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        enemy = entity as Enemy;
    }

    protected override void ChangeToKnockbackState(float knockbackTime)
    {
        throw new System.NotImplementedException();
    }

    /*public bool IsTargetInRangeOf(CombatAbilityWithColliders combatAbility)
    {
        if (enemy.detection.currentTarget == null) return false;

        bool targetInRange = false;

        foreach (OverlapCollider affectedRange in combatAbility.overlapColliders)
        {

        }
    }*/
}
