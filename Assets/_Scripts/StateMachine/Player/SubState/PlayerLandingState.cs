using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLandingState : PlayerState
{
    private Timer landRecoveryTimer;

    public PlayerLandingState(Player player, string animBoolName) : base(player, animBoolName)
    {
        landRecoveryTimer = new Timer(playerData.landingRecoveryTime);
        landRecoveryTimer.timerAction += () => { canTransit = true; };
    }

    public override void Enter()
    {
        base.Enter();

        canTransit = false;
        landRecoveryTimer.StartSingleUseTimer();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        landRecoveryTimer.Tick();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            if (!isGrounded)
            {
                stateMachine.ChangeState(player.inAirState);
                // 예를 들어, 박스 위에 착지했는데 박스가 공격을 받아 부서진 경우
            }

            if (canTransit)
            {
                stateMachine.ChangeState(player.idleState);
            }
        }
    }
}
