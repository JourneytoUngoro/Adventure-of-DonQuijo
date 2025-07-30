using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Police : Enemy
{
    public PoliceIdleState idleState { get; private set; }
    public PoliceMeleeAttackState meleeAttackState { get; private set; }
    public PoliceTargetInDetectionRangeState targetInDetectionRangeState { get; private set; }

    public PoliceCombat policeCombat { get; private set; }

    protected override void Start()
    {
        base.Start();

        policeCombat = entityCombat as PoliceCombat;

        idleState = new PoliceIdleState(this, "idle");
        targetInDetectionRangeState = new PoliceTargetInDetectionRangeState(this, "move");
        meleeAttackState = new PoliceMeleeAttackState(this, "meleeAttack");

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
}
