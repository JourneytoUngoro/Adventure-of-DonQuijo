using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorTargetInDetectionRangeState : EnemyTargetInDetectionRangeState
{
    public Doctor doctor { get; private set; }

    private bool isTargetInRangedAttackRange;

    public DoctorTargetInDetectionRangeState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        doctor = enemy as Doctor;
    }

    public override void Exit()
    {
        base.Exit();

        doctor.animator.SetBool("idle", false);
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isTargetInRangedAttackRange = enemy.combat.IsTargetInRangeOf(doctor.doctorCombat.rangedAttack);
    }

    public override void LateLogicUpdate()
    {
        base.LateLogicUpdate();

        if (!onStateExit)
        {
            if (isGrounded)
            {
                if (!isTargetInDetectionRange)
                {
                    if (enemy.detection.currentTargetLastVelocity.x * facingDirection < 0)
                    {
                        enemy.movement.Flip();
                    }

                    stateMachine.ChangeState(doctor.idleState);
                }
                else if (enemy.movement.navMeshAgentState != NavMeshAgentState.TraverseAround)
                {
                    /*if (enemy.status[(int)CurrentStatus.Alerted])
                    {
                        if (enemy.combat.currentParryStack > 0)
                        {
                            stateMachine.ChangeState(enemy.blockParryState);
                        }
                        else if (enemy.dodgeAttackState.available)
                        {
                            stateMachine.ChangeState(enemy.dodgeAttackState);
                        }
                        else if (enemy.combat.currentBlockStack > 0)
                        {
                            stateMachine.ChangeState(enemy.blockParryState);
                        }
                    }*/

                    if (isTargetInRangedAttackRange && doctor.rangedAttackState.available)
                    {
                        stateMachine.ChangeState(doctor.rangedAttackState);
                    }
                }
            }
        }
    }
}
