using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAbilitySetState : EnemyState
{
    public List<Timer> abilityTimers { get; protected set; } = new List<Timer>();
    public List<bool> availables { get; protected set; } = new List<bool>();

    protected bool isAbilityDone;

    public EnemyAbilitySetState(Enemy enemy, string animBoolName, params float[] cooldowns) : base(enemy, animBoolName)
    {
        foreach (float cooldown in cooldowns)
        {
            abilityTimers.Add(new Timer(cooldown));
            availables.Add(false);
        }
    }

    

    public void Enter(int typeIndex)
    {
        Enter();

        isAbilityDone = false;
        availables[typeIndex] = false;
        enemy.animator.SetInteger("typeIndex", typeIndex);
    }
}
