using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMeleeAttack0State : PlayerAbilityState
{
    public Timer attackComboTimer { get; private set; }
    public int attackStroke { get; set; }

    private Timer attackInputHoldTimer;
    private bool transitToNextAttack;
    private bool canCancel;

    public PlayerMeleeAttack0State(Player player, string animBoolName) : base(player, animBoolName)
    {
        available = true;
        attackComboTimer = new Timer(playerData.attackStrokeTime);
        attackComboTimer.timerAction += () => { attackStroke = 0; };
        attackInputHoldTimer = new Timer(playerData.attackInputBufferTime);
        attackInputHoldTimer.timerAction += () => { transitToNextAttack = false; };
    }

    public override void AnimationStartTrigger(int index)
    {
        base.AnimationStartTrigger(index);
        
        canCancel = false;
    }

    public override void AnimationAlertTrigger(int index)
    {
        base.AnimationAlertTrigger(index);

        player.combat.DoAlert(player.combat.meleeAttack[0]);
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        player.combat.DoAttack(player.combat.meleeAttack[0]);
        player.combat.damagedTargets.Clear();
        Manager.Instance.soundManager.PlaySoundFXClip("playerAttack1SFX", player.transform);
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        if (index == 0)
        {
            canCancel = true;
            canTransit = true;
        }
        else if (index == 1)
        {
            isAbilityDone = true;
        }
    }

    public override void Enter()
    {
        base.Enter();

        canCancel = true;
        transitToNextAttack = false;
        attackInputPressed = false;
        player.animator.SetInteger("typeIndex", 0);
        player.movement.SetVelocityZero();
    }

    public override void Exit()
    {
        base.Exit();

        attackStroke = 1;
        transitToNextAttack = false;
        attackComboTimer.StartSingleUseTimer();
        // player.animator.ResetTrigger("transitToNextAttack");
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
                    stateMachine.ChangeState(player.idleState);
                }
                else
                {
                    stateMachine.ChangeState(player.inAirState);
                }
            }
            else
            {
                if (canCancel)
                {
                    if (dodgeInputPressed && player.dodgeState.available)
                    {
                        stateMachine.ChangeState(player.dodgeState);
                    }
                    else if (blockParryInputPressed && player.blockParryState.available)
                    {
                        stateMachine.ChangeState(player.blockParryState);
                    }
                }
                
                if (canTransit)
                {
                    if (transitToNextAttack)
                    {
                        player.animator.SetTrigger("transitToNextAttack");
                        stateMachine.ChangeState(player.meleeAttack1State);
                    }
                }
            }
        }
    }
}
