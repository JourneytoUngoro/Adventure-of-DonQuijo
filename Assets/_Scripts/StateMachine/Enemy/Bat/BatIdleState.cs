using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatIdleState : EnemyIdleState
{
    private Bat bat;

    public BatIdleState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        bat = enemy as Bat;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (isTargetInDetectionRange && !enemy.detection.currentTarget.isDead)
            {
                stateMachine.ChangeState(bat.targetInDetectionRangeState);
            }
        }
    }
}
