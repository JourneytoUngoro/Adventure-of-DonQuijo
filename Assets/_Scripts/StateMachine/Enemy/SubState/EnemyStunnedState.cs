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
        enemy.stats.posture.ControlRecoveryTimer(TimerControl.Stop);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.stats.posture.SetCurrentValue(enemy.stats.posture.maxValue);
        enemy.stats.posture.ControlRecoveryTimer(TimerControl.Start);
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
                        if (enemy.GetType().Equals(typeof(Bat)))
                        {
                            stateMachine.ChangeState((enemy as Bat).targetInDetectionRangeState);
                        }
                        else if (enemy.GetType().Equals(typeof(Police)))
                        {
                            stateMachine.ChangeState((enemy as Police).targetInDetectionRangeState);
                        }
                        else if (enemy.GetType().Equals(typeof(Doctor)))
                        {
                            stateMachine.ChangeState((enemy as Doctor).targetInDetectionRangeState);
                        }
                    }
                    else
                    {
                        if (enemy.GetType().Equals(typeof(Bat)))
                        {
                            stateMachine.ChangeState((enemy as Bat).idleState);
                        }
                        else if (enemy.GetType().Equals(typeof(Police)))
                        {
                            stateMachine.ChangeState((enemy as Police).idleState);
                        }
                        else if (enemy.GetType().Equals(typeof(Doctor)))
                        {
                            stateMachine.ChangeState((enemy as Doctor).idleState);
                        }
                    }
                }
            }
        }
    }
}
