using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWhirlwindState : PlayerAbilityState
{
    public PlayerWhirlwindState(Player player, string animBoolName) : base(player, animBoolName)
    {
        available = true;
        abilityCoolDownTimer.ChangeDuration(playerData.whirlwindCoolDownTime);
        abilityCoolDownTimer.timerAction += () => { available = true; };
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        player.combat.DoAttack(player.combat.whirlwind[index]);
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
        player.stateMachineToAnimator.state = this;
        // TODO: 감속 매커니즘
    }

    public override void Exit()
    {
        base.Exit();

        abilityCoolDownTimer.StartSingleUseTimer();
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
