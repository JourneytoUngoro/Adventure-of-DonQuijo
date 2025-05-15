using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public Timer dashInputXBufferTimer { get; private set; }
    public Timer dashInputYBufferTimer { get; private set; }
    public bool isDashing { get; private set; }
    private int prevInputX;
    private int prevInputY;

    public PlayerMoveState(Player player, string animBoolName) : base(player, animBoolName)
    {
        dashInputXBufferTimer = new Timer(playerData.dashInputBufferTime);
        dashInputXBufferTimer.timerAction += () => { prevInputX = 0; };
        dashInputYBufferTimer = new Timer(playerData.dashInputBufferTime);
        dashInputYBufferTimer.timerAction += () => { prevInputY = 0; };
    }

    public override void Enter()
    {
        base.Enter();

        if (!isDashing)
        {
            isDashing = (inputX != 0 && prevInputX == inputX) || (inputY != 0 && prevInputY == inputY);
            prevInputX = isDashing ? 0 : inputX;
            prevInputY = isDashing ? 0 : inputY;
        }

        dashInputXBufferTimer.StartSingleUseTimer();
        dashInputYBufferTimer.StartSingleUseTimer();
    }

    public override void Exit()
    {
        base.Exit();

        if (stateMachine.nextState != player.jumpState && stateMachine.nextState != player.idleState && stateMachine.nextState != player.inAirState)
        {
            isDashing = false;
        }

        if (stateMachine.nextState != player.idleState)
        {
            prevInputX = 0;
            prevInputY = 0;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            if (inputX == 0 && inputY == 0)
            {
                stateMachine.ChangeState(player.idleState);
            }
            else if (attackInputPressed && isDashing)
            {
                stateMachine.ChangeState(player.dashAttackState);
            }
            else if (strongAttackInputPressed && isDashing)
            {
                stateMachine.ChangeState(player.strongDashAttackState);
            }
        }

        if (!onStateExit)
        {
            if (isDashing)
            {
                player.animator.SetBool("dash", true);
                player.movement.SetVelocity(inputX * playerData.dashSpeed.x, inputY * playerData.dashSpeed.y);
            }
            else
            {
                player.animator.SetBool("dash", false);
                player.movement.SetVelocity(inputX * playerData.moveSpeed.x, inputY * playerData.moveSpeed.y);
            }
        }
    }

    public void StopDash() => isDashing = false;
}
