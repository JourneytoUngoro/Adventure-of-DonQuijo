using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTouchingWallState : PlayerState
{
    protected bool isTouchingWall;

    public PlayerTouchingWallState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isTouchingWall = player.detection.isTouchingWall();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (isGrounded)
        {
            stateMachine.ChangeState(player.idleState);
        }
        else
        {
            if (!isTouchingWall)
            {
                stateMachine.ChangeState(player.inAirState);
            }
        }
    }
}
