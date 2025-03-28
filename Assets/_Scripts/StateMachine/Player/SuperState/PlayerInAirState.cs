using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInAirState : PlayerState
{
    private Timer coyoteTimer;
    private Timer landingStateTimer;

    private bool isTouchingWall;
    private bool variableJumpHeightAvail;
    private bool gotoLandingState;

    private Trigger fallTrigger;

    public PlayerInAirState(Player player, string animBoolName) : base(player, animBoolName)
    {
        coyoteTimer = new Timer(playerData.coyoteTime);
        coyoteTimer.timerAction += () => { player.jumpState.SetAvailable(false); };
        landingStateTimer = new Timer(playerData.gotoLandingStateTime);
        landingStateTimer.timerAction += () => { gotoLandingState = true; };
    }

    public override void Enter()
    {
        base.Enter();

        isGrounded = false;
        fallTrigger = currentVelocity.z < epsilon ? true : false;
        player.orthogonalRigidbody.gravityScale = playerData.gravityScale;
        coyoteTimer.StartSingleUseTimer();
    }

    public override void Exit()
    {
        base.Exit();

        variableJumpHeightAvail = false;
        Manager.Instance.inputHandler.UnlockMoveInput();
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isTouchingWall = player.detection.isTouchingWall();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            coyoteTimer.Tick();

            landingStateTimer.Tick();

            player.animator.SetFloat("velocityZ", currentVelocity.z);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            if (isGrounded)
            {
                if (gotoLandingState)
                {
                    stateMachine.ChangeState(player.landingState);
                }
                else
                {
                    stateMachine.ChangeState(player.idleState);
                }
            }
            else if (jumpInputPressed)
            {
                if (isTouchingWall)
                {
                    stateMachine.ChangeState(player.wallJumpState);
                }
                else if (player.jumpState.available)
                {
                    stateMachine.ChangeState(player.jumpState);
                }
            }
        }

        if (!onStateExit)
        {
            if (currentVelocity.z < epsilon)
            {
                if (fallTrigger.Value)
                {
                    landingStateTimer.StartSingleUseTimer();
                }
            }
            else
            {
                fallTrigger.Reset();
                gotoLandingState = false;
                landingStateTimer.StopTimer();
            }

            VariableJumpHeight();

            player.movement.CheckIfShouldFlip(inputX);

            if (player.moveState.isDashing)
            {
                player.movement.SetVelocity(inputX * playerData.dashSpeed.x, inputY * playerData.dashSpeed.y);
            }
            else
            {
                player.movement.SetVelocity(inputX * playerData.moveSpeed.x, inputY * playerData.moveSpeed.y);
            }
        }
    }

    private void VariableJumpHeight()
    {
        if (variableJumpHeightAvail)
        {
            if (!jumpInputHolding)
            {
                player.movement.SetVelocityZ(currentVelocity.z * playerData.variableJumpHeightMultiplier);
                variableJumpHeightAvail = false;
            }
            else if (currentVelocity.z < epsilon)
            {
                variableJumpHeightAvail = false;
            }
        }
    }

    public void SetVariableJumpHeight() => variableJumpHeightAvail = true;
}
