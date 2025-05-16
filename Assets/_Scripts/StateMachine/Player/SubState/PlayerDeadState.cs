using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.gameObject.tag = "Invinsible";
    }

    public override void Exit()
    {
        base.Exit();

        player.gameObject.tag = "Idle";
    }
}
