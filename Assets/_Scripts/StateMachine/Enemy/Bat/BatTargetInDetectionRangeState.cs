using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatTargetInDetectionRangeState : EnemyTargetInDetectionRangeState
{
    public Bat bat { get; private set; }

    private bool isTargetInMeleeAttackRange;

    public BatTargetInDetectionRangeState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        bat = enemy as Bat;
    }

    public override void Exit()
    {
        base.Exit();

        bat.animator.SetBool("idle", false);
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isTargetInMeleeAttackRange = enemy.combat.IsTargetInRangeOf(bat.batCombat.meleeAttack);
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

                    stateMachine.ChangeState(bat.idleState);
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

                    if (isTargetInMeleeAttackRange && bat.meleeAttackState.available)
                    {
                        stateMachine.ChangeState(bat.meleeAttackState);
                    }
                }
            }
        }
    }
}
