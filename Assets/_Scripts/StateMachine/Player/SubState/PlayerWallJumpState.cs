using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerTouchingWallState
{
    public PlayerWallJumpState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        player.movement.Flip();
        player.movement.SetVelocityZ(playerData.wallJumpSpeed);
        player.orthogonalRigidbody.gravityScale = playerData.gravityScale;
        player.playerStateMachine.ChangeState(player.inAirState);
    }

    public override void Enter()
    {
        base.Enter();

        Manager.Instance.inputHandler.InactiveJumpInput();
        Manager.Instance.inputHandler.LockMoveInput(new Vector2(-facingDirection, 0.0f));

        isGrounded = false;
        player.movement.SetVelocityZ(0.0f);
        player.orthogonalRigidbody.gravityScale = 0.0f;
        player.stateMachineToAnimator.state = this;
    }

    public override void Exit()
    {
        base.Exit();

        if (stateMachine.nextState != player.inAirState)
        {
            Manager.Instance.inputHandler.UnlockMoveInput();
        }
    }
}
