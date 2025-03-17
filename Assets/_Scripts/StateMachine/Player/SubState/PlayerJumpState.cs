using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public bool canJump { get; private set; }

    public PlayerJumpState(Player player, string animBoolName) : base(player, animBoolName)
    {
        canJump = true;
    }

    public override void Enter()
    {
        base.Enter();

        canJump = false;
        isGrounded = false;
        Manager.Instance.inputHandler.InactiveJumpInput();
        player.inAirState.SetVariableJumpHeight();
        player.movement.SetVelocityY(player.movement.jumpSpeed);
        player.playerStateMachine.ChangeState(player.inAirState);
        player.rigidBody.gravityScale = 1.0f;
    }

    public void CanJump() => canJump = true;
}
