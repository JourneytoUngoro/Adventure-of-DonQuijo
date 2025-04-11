using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.navMeshAgent.enabled = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (isTargetInDetectionRange)
            {
                stateMachine.ChangeState(enemy.targetInDetectionRangeState);
            }
        }
    }
}
