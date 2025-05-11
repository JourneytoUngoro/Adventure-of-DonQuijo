using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeAttack2State : PlayerAbilityState
{
    private Timer attackInputHoldTimer;
    private bool transitToNextAttack;

    public PlayerMeleeAttack2State(Player player, string animBoolName) : base(player, animBoolName)
    {
        available = true;
        attackInputHoldTimer = new Timer(playerData.attackInputBufferTime);
        attackInputHoldTimer.timerAction += () => { transitToNextAttack = false; };
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        player.combat.DoAttack(player.combat.meleeAttack[2]);
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        isAbilityDone = true;
        player.meleeAttack0State.attackComboTimer.StartSingleUseTimer();
    }

    public override void Enter()
    {
        base.Enter();

        isAbilityDone = false;
        transitToNextAttack = false;
        attackInputPressed = false;
        player.stateMachineToAnimator.state = this;
    }

    public override void Exit()
    {
        base.Exit();

        transitToNextAttack = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            attackInputHoldTimer.Tick();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            transitToNextAttack = attackInputPressed ? true : transitToNextAttack;

            if (isAbilityDone)
            {
                if (isGrounded)
                {
                    if (transitToNextAttack)
                    {
                        stateMachine.ChangeState(player.meleeAttack0State);
                    }
                    else
                    {
                        stateMachine.ChangeState(player.idleState);
                    }
                }
                else
                {
                    stateMachine.ChangeState(player.inAirState);
                }
            }
        }
    }
}
