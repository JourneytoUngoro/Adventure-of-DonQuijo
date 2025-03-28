using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerAbilityState
{
    public PlayerJumpState(Player player, string animBoolName) : base(player, animBoolName)
    {
        available = true;
    }

    public override void Enter()
    {
        base.Enter();

        Manager.Instance.inputHandler.InactiveJumpInput();
        player.inAirState.SetVariableJumpHeight();

        isGrounded = false;
        available = false;
        player.movement.SetVelocityZ(playerData.jumpSpeed);
        player.playerStateMachine.ChangeState(player.inAirState);
        player.orthogonalRigidbody.gravityScale = playerData.gravityScale;
    }
}