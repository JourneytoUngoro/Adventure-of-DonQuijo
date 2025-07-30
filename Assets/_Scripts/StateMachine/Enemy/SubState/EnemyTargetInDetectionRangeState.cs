using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTargetInDetectionRangeState : EnemyState
{
    private bool[] meleeAttacks = new bool[3];
    private bool isTargetInMeleeAttack0Range;
    private bool isTargetInMeleeAttack1Range;
    private bool isTargetInMeleeAttack2Range;
    private bool isTargetInDashAttackRange;
    private bool isTargetInWideAttackRange;
    private Timer repositioningTimer;

    private Vector3 currentDestination;
    private Vector3 baseDestinationPosition;
    private Vector2 positionOffset;
    private float traverseDirection;

    private bool traverseAroundFlag;

    public EnemyTargetInDetectionRangeState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        repositioningTimer = new Timer(enemyData.repositioningTime);

        repositioningTimer.timerAction += () =>
        {
            if (UtilityFunctions.RandomSuccess(enemy.enemyData.repositioningPossibility))
            {
                Debug.Log($"{enemy.name} Traverse");
                traverseAroundFlag = false;
                enemy.navMeshAgent.enabled = true;
                enemy.movement.ChangeNavMeshAgentState(NavMeshAgentState.TraverseAround);

                /*positionOffset = Random.insideUnitCircle * enemy.enemyData.repositionOffsetDistance;
                baseDestinationPosition = enemy.detection.currentTarget.entityDetection.currentProjectedPosition;

                traverseDirection = baseDestinationPosition.x > enemy.detection.currentProjectedPosition.x ? 1 : -1;
                currentDestination = baseDestinationPosition + (Vector3)positionOffset;
                currentDestination += UtilityFunctions.RandomSuccess(0.5f) ? Vector3.up * enemy.enemyData.repositionOffsetDistance : Vector3.down * enemy.enemyData.repositionOffsetDistance;
                enemy.navMeshAgent.SetDestination(currentDestination);*/
            }
        };
    }

    public override void Enter()
    {
        base.Enter();

        enemy.navMeshAgent.enabled = true;
        repositioningTimer.StartMultiUseTimer();
        enemy.movement.ChangeNavMeshAgentState(NavMeshAgentState.Chase);
        enemy.detection.currentTarget.entityCombat.targetedBy.Add(enemy);
        positionOffset = Random.insideUnitCircle * enemy.enemyData.repositionOffsetDistance;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.animator.SetBool("idle", false);
        enemy.navMeshAgent.enabled = false;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        /*isTargetInMeleeAttack0Range = enemy.combat.IsTargetInRangeOf(enemy.combat.meleeAttack0[0]);
        isTargetInMeleeAttack1Range = enemy.combat.IsTargetInRangeOf(enemy.combat.meleeAttack1[0]);
        isTargetInMeleeAttack2Range = enemy.combat.IsTargetInRangeOf(enemy.combat.meleeAttack2[0]);
        isTargetInDashAttackRange = enemy.detection.currentTarget != null && Vector3.Distance(enemy.detection.currentProjectedPosition, enemy.detection.currentTarget.entityDetection.currentProjectedPosition) < 400.0f;
        isTargetInWideAttackRange = enemy.combat.IsTargetInRangeOf(enemy.combat.wideRangeAttack);*/
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            repositioningTimer.Tick();
        }
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
    }
}
