using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public event Action playerDeathAction;
    public Timer waitTimer { get; private set; }

    public PlayerDeadState(Player player, string animBoolName) : base(player, animBoolName)
    {
        waitTimer = new Timer(player.playerData.waitTimeAfterDeath);
    }

    public override void Enter()
    {
        base.Enter();

        player.isDead = true;
        waitTimer.StartSingleUseTimer();
        player.gameObject.tag = "Invinsible";
        playerDeathAction?.Invoke();
        Debug.Log("player Dead State Entered");
        player.stats.posture.ControlRecoveryTimer(TimerControl.Stop);
    }

    public override void Exit()
    {
        base.Exit();

        player.gameObject.tag = "Idle";
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            waitTimer.Tick();
        }
    }
}
