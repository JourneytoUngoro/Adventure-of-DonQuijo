using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashAttackState : PlayerAbilityState
{
    public PlayerDashAttackState(Player player, string animBoolName) : base(player, animBoolName)
    {
        available = true;
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        player.combat.DoAttack(player.combat.dashAttack[1]);
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        isAbilityDone = true;
    }

    public override void Enter()
    {
        base.Enter();

        isAbilityDone = false;
        player.stateMachineToAnimator.state = this;
        player.combat.DoAttack(player.combat.dashAttack[0]);
        // Manager.Instance.soundFXManager.PlaySoundFXClip(Manager.Instance.soundFXManager.playerAttackSoundFX, player.transform);
    }

    public override void Exit()
    {
        base.Exit();

        player.combat.damagedTargets.Clear();
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
        }
    }
}
