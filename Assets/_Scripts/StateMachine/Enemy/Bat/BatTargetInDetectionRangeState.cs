using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatTargetInDetectionRangeState : EnemyTargetInDetectionRangeState
{
    public Bat bat { get; private set; }

    private bool isTargetInMeleeAttackRange;

    public BatTargetInDetectionRangeState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isTargetInMeleeAttackRange = enemy.combat.IsTargetInRangeOf(bat.batCombat.meleeAttack);
    }

    public override void LateLogicUpdate()
    {
        base.LateLogicUpdate();

        /*if (!onStateExit)
        {
            if (isGrounded)
            {
                if (!isTargetInDetectionRange)
                {
                    if (enemy.detection.currentTargetLastVelocity.x * facingDirection < 0)
                    {
                        enemy.movement.Flip();
                    }
                    // enemy.detection.currentTarget?.entityCombat.targetedBy.Remove(enemy);
                    stateMachine.ChangeState(enemy.idleState);
                }
                else if (enemy.movement.navMeshAgentState != NavMeshAgentState.TraverseAround)
                {
                    if (enemy.status[(int)CurrentStatus.Alerted])
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
                    }
                    else if (isTargetInWideAttackRange && enemy.wideAttackState.available)
                    {
                        stateMachine.ChangeState(enemy.wideAttackState);
                    }
                    else if (isTargetInDashAttackRange && enemy.dashAttackState.available)
                    {
                        stateMachine.ChangeState(enemy.dashAttackState);
                    }
                    else if (isTargetInWideAttackRange && enemy.wideAttackState.available)
                    {
                        stateMachine.ChangeState(enemy.wideAttackState);
                    }
                    else
                    {
                        meleeAttacks[0] = isTargetInMeleeAttack0Range && enemy.meleeAttack0State.available;
                        meleeAttacks[1] = isTargetInMeleeAttack1Range && enemy.meleeAttack1State.available;
                        meleeAttacks[2] = isTargetInMeleeAttack2Range && enemy.meleeAttack2State.available;

                        int? meleeAttackType = UtilityFunctions.RandomTrueIndex(meleeAttacks);

                        if (meleeAttackType.HasValue)
                        {
                            switch (meleeAttackType.Value)
                            {
                                case 0:
                                    stateMachine.ChangeState(enemy.meleeAttack0State); break;
                                case 1:
                                    stateMachine.ChangeState(enemy.meleeAttack1State); break;
                                case 2:
                                    stateMachine.ChangeState(enemy.meleeAttack2State); break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }*/

        if (!onStateExit)
        {
            if (!isTargetInDetectionRange)
            {
                if (enemy.detection.currentTargetLastVelocity.x * facingDirection < 0)
                {
                    enemy.movement.Flip();
                }
                // enemy.detection.currentTarget?.entityCombat.targetedBy.Remove(enemy);
                stateMachine.ChangeState(enemy.idleState);
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
                
                if (isTargetInMeleeAttackRange)
                {
                    stateMachine.ChangeState(bat.meleeAttackState);
                }
            }
        }
    }
}
