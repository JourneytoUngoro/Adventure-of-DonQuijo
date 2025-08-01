using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Bat : Enemy
{
    public BatIdleState idleState { get; private set; }
    public BatMeleeAttackState meleeAttackState { get; private set; }
    public BatTargetInDetectionRangeState targetInDetectionRangeState { get; private set; }

    public BatCombat batCombat { get; private set; }

    protected override void Start()
    {
        base.Start();
        
        batCombat = entityCombat as BatCombat;

        idleState = new BatIdleState(this, "idle");
        targetInDetectionRangeState = new BatTargetInDetectionRangeState(this, "move");
        meleeAttackState = new BatMeleeAttackState(this, "meleeAttack");

        /*IEnumerable<PropertyInfo> abilityStateProperties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.PropertyType.IsSubclassOf(typeof(EnemyAbilityState)));

        foreach (PropertyInfo property in abilityStateProperties)
        {
            EnemyAbilityState abilityState = property.GetValue(this) as EnemyAbilityState;
            abilityStates.Add(abilityState);
        }*/

        enemyStateMachine.Initialize(idleState);
    }

    public override void Revive()
    {
        base.Revive();

        enemyStateMachine.Initialize(idleState);
    }
}
