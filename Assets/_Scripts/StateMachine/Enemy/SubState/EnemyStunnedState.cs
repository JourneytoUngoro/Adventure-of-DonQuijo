using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStunnedState : EnemyState
{
    private Timer stunRecoveryTimer;

    public EnemyStunnedState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        stunRecoveryTimer = new Timer(enemyData.stunRecoveryTime);
        stunRecoveryTimer.timerAction += () => { canTransit = true; };
    }

    public override void Enter()
    {
        base.Enter();

        canTransit = false;
        stunRecoveryTimer.StartSingleUseTimer();
    }

    public override void Exit()
    {
        base.Exit();

        enemy.stats.posture.SetCurrentValue(enemy.stats.posture.maxValue);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            stunRecoveryTimer.Tick(isGrounded);

            if (canTransit)
            {
                if (isGrounded)
                {
                    if (isTargetInDetectionRange)
                    {
                        stateMachine.ChangeState(enemy.targetInDetectionRangeState);
                    }
                    else
                    {
                        stateMachine.ChangeState(enemy.idleState);
                    }
                }
            }
        }
    }
}
