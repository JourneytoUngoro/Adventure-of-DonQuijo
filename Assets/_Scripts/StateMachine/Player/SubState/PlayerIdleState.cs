using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    private Timer dashMaintainTimer; // dash 상태에서 좌우로 전환시 짧은 시간 동안 idle이 되면서 dash 상태가 풀리는 현상을 방지

    public PlayerIdleState(Player player, string animBoolName) : base(player, animBoolName)
    {
        dashMaintainTimer = new Timer(playerData.dashMaintainTime);
        dashMaintainTimer.timerAction += () => { player.moveState.StopDash(); };
    }

    public override void Enter()
    {
        base.Enter();

        player.movement.SetVelocityZero();
        dashMaintainTimer.StartSingleUseTimer();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            dashMaintainTimer.Tick();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            if (inputX != 0 || inputY != 0)
            {
                stateMachine.ChangeState(player.moveState);
            }
        }
    }
}
