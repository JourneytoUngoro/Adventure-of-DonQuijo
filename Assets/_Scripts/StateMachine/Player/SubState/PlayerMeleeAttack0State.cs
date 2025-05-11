using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeAttack0State : PlayerAbilityState
{
    public Timer attackComboTimer { get; private set; }
    public int attackStroke { get; private set; }

    private Timer attackInputHoldTimer;
    private bool transitToNextAttack;

    public PlayerMeleeAttack0State(Player player, string animBoolName) : base(player, animBoolName)
    {
        available = true;
        attackComboTimer = new Timer(playerData.attackStrokeTime);
        attackComboTimer.timerAction += () => { attackStroke = 0; };
        attackInputHoldTimer = new Timer(playerData.attackInputBufferTime);
        attackInputHoldTimer.timerAction += () => { transitToNextAttack = false; };
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        player.combat.DoAttack(player.combat.meleeAttack[0]);
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        isAbilityDone = true;
        attackComboTimer.StartSingleUseTimer();
    }

    public override void Enter()
    {
        base.Enter();

        isAbilityDone = false;
        transitToNextAttack = false;
        attackInputPressed = false;
        player.stateMachineToAnimator.state = this;
        Manager.Instance.soundFXManager.PlaySoundFXClip(Manager.Instance.soundFXManager.playerAttackSoundFX, player.transform);
    }

    public override void Exit()
    {
        base.Exit();

        transitToNextAttack = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            attackInputHoldTimer.Tick();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            transitToNextAttack = attackInputPressed ? true : transitToNextAttack;

            if (isAbilityDone)
            {
                if (isGrounded)
                {
                    if (transitToNextAttack)
                    {
                        stateMachine.ChangeState(player.meleeAttack1State);
                    }
                    else
                    {
                        stateMachine.ChangeState(player.idleState);
                    }
                }
                else
                {
                    stateMachine.ChangeState(player.inAirState);
                }
            }
        }
    }
}
