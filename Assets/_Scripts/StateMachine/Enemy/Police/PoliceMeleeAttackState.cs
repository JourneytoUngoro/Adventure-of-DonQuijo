using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceMeleeAttackState : EnemyAbilityState
{
    private Police police;

    public PoliceMeleeAttackState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        police = enemy as Police;
        available = true;
        abilityCoolDownTimer.ChangeDuration(enemyData.meleeAttack0CoolDown);
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        enemy.combat.DoAttack(police.policeCombat.meleeAttack[index]);

        Manager.Instance.soundManager.PlaySoundFXClip("policeAttackSFX", enemy.transform);
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        isAbilityDone = true;
    }

    public override void Enter()
    {
        base.Enter();

        available = false;
        enemy.movement.SetVelocityZero();
        police.animator.SetBool("idle", false);
    }

    public override void Exit()
    {
        base.Exit();

        abilityCoolDownTimer.StartSingleUseTimer();
        enemy.combat.damagedTargets.Clear();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (isAbilityDone)
            {
                if (isGrounded)
                {
                    if (isTargetInDetectionRange)
                    {
                        stateMachine.ChangeState(police.targetInDetectionRangeState);
                    }
                    else
                    {
                        if (enemy.detection.currentTargetLastVelocity.x * facingDirection < 0)
                        {
                            enemy.movement.Flip();
                        }
                        stateMachine.ChangeState(police.idleState);
                    }
                }
                else
                {
                    // stateMachine.ChangeState(enemy.inAirState);
                }
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            if (!isAbilityDone)
            {
                if ((enemy.detection.currentTarget.entityDetection.currentProjectedPosition.x - currentProjectedPosition.x) * facingDirection < 0)
                {
                    enemy.movement.Flip();
                }
            }
        }
    }
}
