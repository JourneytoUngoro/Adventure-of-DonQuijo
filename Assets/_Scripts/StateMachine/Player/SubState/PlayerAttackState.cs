using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerAbilityState
{
    public Timer attackComboTimer { get; private set; }
    private Timer attackInputHoldTimer;
    private bool transitToNextAttack;
    private int attackStroke;

    public PlayerAttackState(Player player, string animBoolName) : base(player, animBoolName)
    {
        attackComboTimer = new Timer(playerData.attackStrokeTime);
        attackComboTimer.timerAction += () => { attackStroke = 0; };
        attackInputHoldTimer = new Timer(playerData.attackInputBufferTime);
        attackInputHoldTimer.timerAction += () => { transitToNextAttack = false; };
    }

    public override void AnimationStartTrigger(int index)
    {
        base.AnimationStartTrigger(index);

        transitToNextAttack = false;
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        player.combat.DoAttack(player.combat.meleeAttack[attackStroke]);
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        isAbilityDone = true;
        attackComboTimer.StartSingleUseTimer();
        player.animator.SetInteger("typeIndex", (attackStroke + 1) % player.combat.meleeAttack.Count); ;
    }

    public override void Enter()
    {
        base.Enter();

        isAbilityDone = false;
        transitToNextAttack = false;
    }

    public override void Exit()
    {
        base.Exit();

        transitToNextAttack = false;
    }

    public override void LateLogicUpdate()
    {
        base.LateLogicUpdate();

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

            if (attackInputPressed)
            {
                attackInputHoldTimer.StartSingleUseTimer();
            }

            if (isAbilityDone)
            {
                if (isGrounded)
                {
                    if (transitToNextAttack)
                    {
                        stateMachine.ChangeState(player.attackState);
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
