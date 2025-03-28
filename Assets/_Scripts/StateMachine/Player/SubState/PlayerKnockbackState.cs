using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockbackState : PlayerState
{
    public Timer knockbackTimer { get; private set; }

    private bool shouldTransitToStunnedState;

    public PlayerKnockbackState(Player player, string animBoolName) : base(player, animBoolName)
    {
        knockbackTimer = new Timer(0.0f);
        knockbackTimer.timerAction += () => { canTransit = true; };
    }

    public void Enter(float stunnedTime)
    {
        Enter();

        canTransit = false;
        knockbackTimer.ChangeDuration(stunnedTime);
        knockbackTimer.StartSingleUseTimer();
        shouldTransitToStunnedState = player.stats.posture.currentValue == player.stats.posture.maxValue;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (canTransit)
            {
                if (shouldTransitToStunnedState)
                {
                    stateMachine.ChangeState(player.stunnedState);
                }
                else
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

    public void TransitedToStunnedState() => shouldTransitToStunnedState = false;
}
