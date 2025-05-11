using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKnockbackState : EnemyState
{
    protected bool shouldTransitToStunnedState;
    protected Vector2 knockbackVelocity;

    public Timer knockbackTimer { get; private set; }

    public EnemyKnockbackState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        knockbackTimer = new Timer(0.0f);
        knockbackTimer.timerAction += () => { canTransit = true; };
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        canTransit = true;
    }

    public override void Enter()
    {
        base.Enter();

        canTransit = false;
        knockbackTimer.StartSingleUseTimer();
        enemy.stateMachineToAnimator.state = this;
        // 아직 사운드 준비 안 됨 
        // Manager.Instance.soundManager.PlaySoundFXClip("", enemy.transform);

    }

    public override void Exit()
    {
        base.Exit();



        if (canTransit)
        {
            shouldTransitToStunnedState = false;
            enemy.stats.posture.SetCurrentValue(enemy.stats.posture.minValue);
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (canTransit)
            {
                if (shouldTransitToStunnedState)
                {
                    stateMachine.ChangeState(enemy.stunnedState);
                }
                else if (isTargetInDetectionRange)
                {
                    stateMachine.ChangeState(enemy.targetInDetectionRangeState);
                }
                /*else
                {
                    stateMachine.ChangeState(enemy.lookForTargetState);
                }*/
            }
        }
    }

    public void ShouldTransitToStunnedState() => shouldTransitToStunnedState = true;
    public void SetKnockbackVelocity(Vector2 knockbackVelocity) => this.knockbackVelocity = knockbackVelocity;
}
