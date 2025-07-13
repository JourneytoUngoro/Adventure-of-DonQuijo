using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRangedAttackState : PlayerAbilityState
{
    public PlayerRangedAttackState(Player player, string animBoolName) : base(player, animBoolName)
    {
        available = true;
        abilityCoolDownTimer.ChangeDuration(playerData.rangedAttackCoolDownTime);
        abilityCoolDownTimer.timerAction += () => { available = true; };
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        player.combat.DoAttack(player.combat.rangedAttack[0]);
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
