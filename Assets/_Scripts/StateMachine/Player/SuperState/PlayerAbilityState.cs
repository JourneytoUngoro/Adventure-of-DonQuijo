using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityState : PlayerState
{
    public Timer abilityCoolDownTimer { get; protected set; }
    public bool available { get; protected set; }

    protected bool isAbilityDone;

    public PlayerAbilityState(Player player, string animBoolName) : base(player, animBoolName)
    {
        abilityCoolDownTimer = new Timer(0.0f);
        abilityCoolDownTimer.timerAction += () => { available = true; };
    }

    public override void Enter()
    {
        base.Enter();

        isAbilityDone = false;
        player.stateMachineToAnimator.state = this;
        player.stats.posture.ControlRecoveryTimer(TimerControl.Stop);
    }

    public override void Exit() 
    {
        base.Exit();

        player.stats.posture.ControlRecoveryTimer(TimerControl.Start);
    }

    public void SetAvailable(bool available) => this.available = available;
}
