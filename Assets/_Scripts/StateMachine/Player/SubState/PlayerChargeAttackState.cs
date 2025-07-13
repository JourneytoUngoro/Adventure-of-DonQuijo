using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChargeAttackState : PlayerAbilityState
{
    private bool startDashAttack;

    public PlayerChargeAttackState(Player player, string animBoolName) : base(player, animBoolName)
    {
        available = true;
        abilityCoolDownTimer.ChangeDuration(playerData.chargeAttackCoolDownTime);
        abilityCoolDownTimer.timerAction += () => { available = true; };
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        startDashAttack = true;
        player.combat.DoAttack(player.combat.dashAttack[0]);
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        isAbilityDone = true;
    }

    public override void Enter()
    {
        base.Enter();

        available = false;
        startDashAttack = false;
        player.combat.SetStanceLevel(1);
        player.movement.SetVelocityZero();
    }

    public override void Exit()
    {
        base.Exit();

        abilityCoolDownTimer.StartSingleUseTimer();
        player.combat.damagedTargets.Clear();
        player.movement.StopVelocityChangeOverTime();
        player.movement.SetVelocityZero();
        player.combat.SetStanceLevel(0);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            if (startDashAttack)
            {
                player.combat.DoAttack(player.combat.chargeAttack[1]);
            }

            if (isAbilityDone)
            {
                if (isGrounded)
                {
                    stateMachine.ChangeState(player.idleState);
                }
                else
                {
                    stateMachine.ChangeState(player.inAirState);
                }
            }
        }
    }
}
