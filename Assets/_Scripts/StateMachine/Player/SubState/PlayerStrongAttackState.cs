using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStrongAttackState : PlayerAbilityState
{
    private bool canCancel;

    public PlayerStrongAttackState(Player player, string animBoolName) : base(player, animBoolName)
    {
        available = true;
    }

    public override void AnimationStartTrigger(int index)
    {
        base.AnimationStartTrigger(index);

        canCancel = false;
    }

    public override void AnimationAlertTrigger(int index)
    {
        base.AnimationAlertTrigger(index);

        player.combat.DoAlert(player.combat.strongAttack);
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        if (index < 0)
        {
            player.combat.DoAlert(player.combat.strongAttack);
        }
        else
        {
            player.combat.DoAttack(player.combat.strongAttack);
            player.combat.damagedTargets.Clear();
            Manager.Instance.soundManager.PlaySoundFXClip("heavyAttackSFX", player.transform);
        }
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
        player.movement.SetVelocityZero();
        // Manager.Instance.soundFXManager.PlaySoundFXClip(Manager.Instance.soundFXManager.playerAttackSoundFX, player.transform);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
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
                    if (attackInputPressed)
                    {
                        if (player.meleeAttack0State.attackStroke == 0)
                        {
                            stateMachine.ChangeState(player.meleeAttack0State);
                        }
                        else if (player.meleeAttack0State.attackStroke == 1)
                        {
                            stateMachine.ChangeState(player.meleeAttack1State);
                        }
                        else if (player.meleeAttack0State.attackStroke == 2)
                        {
                            stateMachine.ChangeState(player.meleeAttack2State);
                        }
                    }
                }
            }
        }
    }
}
