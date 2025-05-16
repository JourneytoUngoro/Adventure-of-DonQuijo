using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAbilityState : EnemyState
{
    public Timer abilityCoolDownTimer { get; protected set; }
    public bool available { get; protected set; }

    protected bool isAbilityDone;

    public EnemyAbilityState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        abilityCoolDownTimer = new Timer(0.0f);
        abilityCoolDownTimer.timerAction += () => { available = true; };
    }

    public override void Enter()
    {
        base.Enter();

        isAbilityDone = false;
        entity.stateMachineToAnimator.state = this;
        enemy.stats.posture.ControlRecoveryTimer(TimerControl.Stop);
    }

    public override void Exit()
    {
        base.Exit();

        isAbilityDone = true;
        enemy.stats.posture.ControlRecoveryTimer(TimerControl.Start);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (isAbilityDone)
        {
            /*if (isGrounded)
            {
                stateMachine.ChangeState(enemy.idleState);
            }
            else
            {
                stateMachine.ChangeState(enemy.inAirState);
            }*/
        }
    }

    public void SetAvailable(bool available) => this.available = available;
}
