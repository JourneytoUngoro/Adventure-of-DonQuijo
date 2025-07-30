using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doctor : Enemy
{
    public DoctorIdleState idleState { get; private set; }
    public DoctorRangedAttackState rangedAttackState { get; private set; }
    public DoctorTargetInDetectionRangeState targetInDetectionRangeState { get; private set; }

    public DoctorCombat doctorCombat { get; private set; }

    protected override void Start()
    {
        base.Start();

        doctorCombat = entityCombat as DoctorCombat;

        idleState = new DoctorIdleState(this, "idle");
        targetInDetectionRangeState = new DoctorTargetInDetectionRangeState(this, "move");
        rangedAttackState = new DoctorRangedAttackState(this, "meleeAttack");

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
