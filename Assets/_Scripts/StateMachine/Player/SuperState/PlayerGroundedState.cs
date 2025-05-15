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

        Manager.Instance.inputHandler.UnlockMoveInput();
        player.jumpState.SetAvailable(true);
        player.orthogonalRigidbody.gravityScale = 0.0f;
        player.movement.SetVelocityY(0.0f);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            /*if (!isGrounded)
            {
                stateMachine.ChangeState(player.inAirState);
            }
            else if (jumpInputPressed && player.jumpState.available)
            {
                stateMachine.ChangeState(player.jumpState);
            }
            else */if (dodgeInputPressed && player.dodgeState.available)
            {
                stateMachine.ChangeState(player.dodgeState);
            }
            else if (blockParryInputPressed && player.blockParryState.available)
            {
                stateMachine.ChangeState(player.blockParryState);
            }
            else if (itemInputPressed && player.cureState.available)
            {
                stateMachine.ChangeState(player.cureState);
            }
            else if (attackInputPressed && !(stateMachine.currentState.Equals(player.moveState) && player.moveState.isDashing))
            {
                if (player.meleeAttack0State.attackStroke == 0)
                {
                    stateMachine.ChangeState(player.meleeAttack0State);
                }
                else if (player.meleeAttack0State.attackStroke == 1)
                {
                    stateMachine.ChangeState(player.meleeAttack1State);
                }
                else if (player.meleeAttack0State.attackStroke == 2)
                {
                    stateMachine.ChangeState(player.meleeAttack2State);
                }
            }
            else if (strongAttackInputPressed && !(stateMachine.currentState.Equals(player.moveState) && player.moveState.isDashing))
            {
                stateMachine.ChangeState(player.strongAttackState);
            }
        }

        if (!onStateExit)
        {
            player.entityMovement.CheckIfShouldFlip(inputX);
        }
    }
}
