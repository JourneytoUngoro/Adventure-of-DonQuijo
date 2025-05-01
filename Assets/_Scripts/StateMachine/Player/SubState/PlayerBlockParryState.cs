using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlockParryState : PlayerAbilityState
{
    private bool isParried;
    private bool isBlocked;

    public PlayerBlockParryState(Player player, string animBoolName) : base(player, animBoolName)
    {
        available = true;
    }

    public override void AnimationStartTrigger(int index)
    {
        base.AnimationStartTrigger(index);

        canTransit = false;

        if (index == 0)
        {
            isBlocked = true;
            player.animator.ResetTrigger("blockParryButtonPressed");
        }
        else if (index == 1)
        {
            isParried = true;
        }
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        canTransit = true;
        available = false;

        if (index == 0)
        {
            isBlocked = false;
        }
        else if (index == 1)
        {
            isParried = false;
        }
    }

    public override void Enter()
    {
        base.Enter();

        player.stateMachineToAnimator.state = this;
        player.animator.SetBool("inAir", !isGrounded);

        available = false;
        player.movement.SetVelocity(0.0f, 0.0f);
        player.entityCombat.DoAttack(player.combat.blockParry);
    }

    public override void Exit()
    {
        base.Exit();

        isParried = false;
        isBlocked = false;
        available = true;
        player.animator.SetBool("inAir", !isGrounded);
        abilityCoolDownTimer.StartSingleUseTimer();
        player.combat.DisableBlockParryPrefabs();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            if (isParried && isGrounded && blockParryInputPressed)
            {
                player.animator.SetTrigger("blockParryInputPressed");
                player.combat.DoAttack(player.combat.blockParry);
            }
            else if (!blockParryInputHolding)
            {
                isAbilityDone = canTransit;
            }
        }

        if (!onStateExit)
        {
            if (isGrounded)
            {
                if (isParried || isBlocked)
                {
                    if (player.detection.IsDetectingLedge(CheckPositionAxis.Horizontal, CheckPositionDirection.Back) || player.detection.IsDetectingLedge(CheckPositionAxis.Horizontal, CheckPositionDirection.Front))
                    {
                        player.movement.SetVelocityX(0.0f);
                    }
                }
            }
        }
    }
}
