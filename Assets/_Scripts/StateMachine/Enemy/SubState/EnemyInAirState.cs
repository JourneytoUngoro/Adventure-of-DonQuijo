using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInAirState : EnemyState
{
    public EnemyInAirState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {

        }
    }
}
