using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.shadow.enabled = false; // Disabling the collider because it stops rigidbody moving downward.
        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - player.transform.position.z + currentGroundHeight, currentGroundHeight);
        player.jumpState.CanJump();
        player.rigidBody.gravityScale = 0.0f;
        player.movement.SetVelocityY(0.0f);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            if (jumpInputPressed && player.jumpState.canJump)
            {
                stateMachine.ChangeState(player.jumpState);
            }
            else if (!isGrounded)
            {
                stateMachine.ChangeState(player.inAirState);
                player.inAirState.prevEntityHeight = currentEntityHeight;
            }
        }

        if (!onStateExit)
        {
            player.entityMovement.CheckIfShouldFlip(inputX);
        }
    }
}
