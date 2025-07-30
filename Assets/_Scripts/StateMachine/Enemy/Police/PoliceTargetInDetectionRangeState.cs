using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceTargetInDetectionRangeState : EnemyTargetInDetectionRangeState
{
    public Police police { get; private set; }

    private bool isTargetInMeleeAttackRange;

    public PoliceTargetInDetectionRangeState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        police = enemy as Police;
    }

    public override void Exit()
    {
        base.Exit();

        police.animator.SetBool("idle", false);
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isTargetInMeleeAttackRange = enemy.combat.IsTargetInRangeOf(police.policeCombat.meleeAttack);
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

                    stateMachine.ChangeState(police.idleState);
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

                    if (isTargetInMeleeAttackRange && police.meleeAttackState.available)
                    {
                        stateMachine.ChangeState(police.meleeAttackState);
                    }
                }
            }
        }
    }
}
