using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Bat : Enemy
{
    public BatMeleeAttackState meleeAttackState { get; private set; }

    public BatCombat batCombat { get; private set; }

    protected override void Start()
    {
        base.Start();
        Debug.Log("Bat");
        batCombat = entityCombat as BatCombat;

        meleeAttackState = new BatMeleeAttackState(this, "meleeAttack");

        abilityStates = new List<EnemyAbilityState>();
        IEnumerable<PropertyInfo> abilityStateProperties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.PropertyType.IsSubclassOf(typeof(EnemyAbilityState)));

        foreach (PropertyInfo property in abilityStateProperties)
        {
            EnemyAbilityState abilityState = property.GetValue(this) as EnemyAbilityState;
            abilityStates.Add(abilityState);
        }
    }
}
