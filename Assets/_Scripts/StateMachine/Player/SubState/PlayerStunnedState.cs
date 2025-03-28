using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStunnedState : PlayerState
{
    private Timer stunRecoveryTimer;

    public PlayerStunnedState(Player player, string animBoolName) : base(player, animBoolName)
    {
        stunRecoveryTimer = new Timer(playerData.stunRecoveryTime);
        stunRecoveryTimer.timerAction += () => { canTransit = true; };
    }

    public override void Enter()
    {
        base.Enter();

        canTransit = false;
        stunRecoveryTimer.StartSingleUseTimer();
        player.knockbackState.TransitedToStunnedState();
    }

    public override void Exit()
    {
        base.Exit();

        player.stats.posture.SetCurrentValue(player.stats.posture.minValue);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            stunRecoveryTimer.Tick();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            if (canTransit)
            {
                if (isGrounded)
                {
                    stateMachine.ChangeState(player.idleState);
                }
                else
                {
                    stateMachine.ChangeState(player.inAirState);
                }
            }
        }
    }
}
