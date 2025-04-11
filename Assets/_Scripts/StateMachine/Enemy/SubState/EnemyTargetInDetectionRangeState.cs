using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetInDetectionRangeState : EnemyState
{
    public EnemyTargetInDetectionRangeState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.navMeshAgent.enabled = true;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.navMeshAgent.enabled = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (!isTargetInDetectionRange)
            {
                stateMachine.ChangeState(enemy.idleState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            
        }
    }
}
