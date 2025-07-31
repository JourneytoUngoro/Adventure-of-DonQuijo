using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorRangedAttackState : EnemyAbilityState
{
    private Doctor doctor;

    public DoctorRangedAttackState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        doctor = enemy as Doctor;
        available = true;
        abilityCoolDownTimer.ChangeDuration(enemyData.meleeAttack0CoolDown);
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        enemy.combat.DoAttack(doctor.doctorCombat.rangedAttack[index]);
        enemy.combat.damagedTargets.Clear();

        Manager.Instance.soundManager.PlaySoundFXClip("doctorAttackSFX", enemy.transform);

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
        doctor.animator.SetBool("idle", false);
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
                        stateMachine.ChangeState(doctor.targetInDetectionRangeState);
                    }
                    else
                    {
                        if (enemy.detection.currentTargetLastVelocity.x * facingDirection < 0)
                        {
                            enemy.movement.Flip();
                        }
                        stateMachine.ChangeState(doctor.idleState);
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
