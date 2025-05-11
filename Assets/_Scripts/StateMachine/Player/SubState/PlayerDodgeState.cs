using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodgeState : PlayerAbilityState
{
    public PlayerDodgeState(Player player, string animBoolName) : base(player, animBoolName)
    {
        available = true;
        abilityCoolDownTimer = new Timer(playerData.dodgeCoolDownTime);
        abilityCoolDownTimer.timerAction += () => { available = true; };
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        player.orthogonalRigidbody.gameObject.tag = "Idle";
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        if (index == 0)
        {
            isAbilityDone = !isGrounded;
        }
        else if (index == 1)
        {
            isAbilityDone = true;
        }
    }

    public override void Enter()
    {
        base.Enter();

        available = false;
        player.stateMachineToAnimator.state = this;
        player.orthogonalRigidbody.gameObject.tag = "Dodge";

        if (inputX == 0 && inputY == 0)
        {
            player.animator.SetBool("backstep", true);
            player.movement.SetVelocityXChangeOverTime(playerData.backstepSpeed * -facingDirection, playerData.backstepTime, Ease.InCubic, true, false);
        }
        else
        {
            player.movement.SetVelocityChangeOverTime(new Vector2(inputX, inputY), playerData.dodgeSpeed, playerData.dodgeTime, Ease.InSine, true, false, playerData.moveSpeed);
        }
    }

    public override void Exit()
    {
        base.Exit();

        player.orthogonalRigidbody.gameObject.tag = "Idle";
        player.animator.SetBool("backstep", false);
        abilityCoolDownTimer.StartSingleUseTimer();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

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
