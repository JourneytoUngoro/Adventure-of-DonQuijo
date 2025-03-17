using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodgeState : PlayerGroundedState // Need to be changed to PlayerAbilityState
{
    public bool canDodge { get; private set; }
    public Timer dodgeCoolDownTimer { get; private set; }

    public PlayerDodgeState(Player player, string animBoolName) : base(player, animBoolName)
    {
        canDodge = true;
        dodgeCoolDownTimer = new Timer(2.0f);
        dodgeCoolDownTimer.timerAction += () => { canDodge = true; };
    }

    public override void Enter()
    {
        base.Enter();

        canDodge = false;
        player.gameObject.tag = "Dodge";
        player.stateMachineToAnimator.state = this;
        if (inputX == 0)
        {
            player.animator.SetTrigger("backstep");
        }
    }

    public override void Exit()
    {
        base.Exit();

        dodgeCoolDownTimer.StartSingleUseTimer();
        player.gameObject.tag = "Idle";
        player.animator.ResetTrigger("backstep");
    }
}
