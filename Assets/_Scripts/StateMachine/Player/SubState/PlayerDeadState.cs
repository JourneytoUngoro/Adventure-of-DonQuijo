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

        player.gameObject.tag = "Invinsible";
        playerDeathAction?.Invoke();
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
