using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInAirState : PlayerState
{
    public float prevEntityHeight;
    private bool variableJumpHeight;
    private float variableJumpHeightMultiplier = 0.5f;
    private Vector3 initialPosition;

    public PlayerInAirState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        isGrounded = false;
        player.rigidBody.gravityScale = 1.0f;
        prevEntityHeight = currentGroundHeight;
        initialPosition = player.transform.position;
        player.shadow.enabled = true;
    }

    public override void Exit()
    {
        base.Exit();

        variableJumpHeight = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            player.animator.SetFloat("velocityZ", currentVelocity.y);

            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, Mathf.Max(player.transform.position.y - initialPosition.y + prevEntityHeight, currentGroundHeight));
        }
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
            else if (isGrounded)
            {
                if (inputX != 0 || inputY != 0)
                {
                    stateMachine.ChangeState(player.moveState);
                }
                else
                {
                    stateMachine.ChangeState(player.idleState);
                }
            }
        }

        if (!onStateExit)
        {
            VariableJumpHeight();

            player.movement.CheckIfShouldFlip(inputX);

            if (facingObstacleHeight.x <= currentEntityHeight)
            {
                player.movement.SetVelocityX(inputX * player.movement.horizontalSpeed);
            }
            else
            {
                player.movement.SetVelocityX(0.0f);
            }

            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, Mathf.Max(player.transform.position.y - initialPosition.y + prevEntityHeight, currentGroundHeight));
        }
    }

    private void VariableJumpHeight()
    {
        if (variableJumpHeight)
        {
            if (!jumpInputHolding)
            {
                player.movement.SetVelocityY(currentVelocity.y * variableJumpHeightMultiplier);
                variableJumpHeight = false;
            }
            else if (currentVelocity.y < epsilon)
            {
                variableJumpHeight = false;
            }
        }
    }

    public void SetVariableJumpHeight() => variableJumpHeight = true;
}
