using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceIdleState : EnemyIdleState
{
    private Police police;

    public PoliceIdleState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        police = enemy as Police;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (isTargetInDetectionRange && !enemy.detection.currentTarget.isDead)
            {
                stateMachine.ChangeState(police.targetInDetectionRangeState);
            }
        }
    }
}