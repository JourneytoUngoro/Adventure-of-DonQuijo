using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBlockParryState : EnemyAbilityState
{
    public EnemyBlockParryState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        available = true;
    }

    public override void Enter()
    {
        base.Enter();

        available = false;
    }
}
