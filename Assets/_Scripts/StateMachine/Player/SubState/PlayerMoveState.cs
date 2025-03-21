using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // inputX = -1;

        if (!onStateExit)
        {
            if (inputX == 0 && inputY == 0)
            {
                stateMachine.ChangeState(player.idleState);
            }
        }

        if (!onStateExit)
        {
            player.movement.SetVelocity(inputX * player.movement.horizontalSpeed, inputY * player.movement.verticalSpeed);
        }
    }
}
