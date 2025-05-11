using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTargetInDetectionRangeState : EnemyState
{
    private bool[] meleeAttacks = new bool[4];
    private bool isTargetInMeleeAttack0Range;
    private bool isTargetInMeleeAttack1Range;
    private bool isTargetInMeleeAttack2Range;
    private bool isTargetInMeleeAttack3Range;
    private Timer repositioningTimer;

    private Vector3 currentDestination;
    private Vector3 baseDestinationPosition;
    private Vector2 positionOffset;
    private float traverseDirection;

    private bool traverseAroundFlag;

    private NavMeshAgentState navMeshAgentState;

    public EnemyTargetInDetectionRangeState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        repositioningTimer = new Timer(enemyData.repositioningTime);
        repositioningTimer.timerAction += () =>
        {
            if (UtilityFunctions.RandomSuccess(enemy.enemyData.repositioningPossibility))
            {
                traverseAroundFlag = false;
                enemy.navMeshAgent.enabled = true;
                navMeshAgentState = NavMeshAgentState.TraverseAround;
                positionOffset = Random.insideUnitCircle * enemy.enemyData.repositionOffsetDistance;
                baseDestinationPosition = enemy.detection.currentTarget.entityDetection.currentProjectedPosition;

                traverseDirection = baseDestinationPosition.x > enemy.detection.currentProjectedPosition.x ? 1 : -1;
                currentDestination = baseDestinationPosition + (Vector3)positionOffset;
                currentDestination += UtilityFunctions.RandomSuccess(0.5f) ? Vector3.up * enemy.enemyData.repositionOffsetDistance : Vector3.down * enemy.enemyData.repositionOffsetDistance;
                enemy.navMeshAgent.SetDestination(currentDestination);
            }
        };
    }

    public override void Enter()
    {
        base.Enter();

        enemy.navMeshAgent.enabled = true;
        repositioningTimer.StartMultiUseTimer();
        navMeshAgentState = NavMeshAgentState.Chase;
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

        isTargetInMeleeAttack0Range = enemy.combat.IsTargetInRangeOf(enemy.combat.meleeAttack0[0]);
        isTargetInMeleeAttack1Range = enemy.combat.IsTargetInRangeOf(enemy.combat.meleeAttack1[0]);
        isTargetInMeleeAttack2Range = enemy.combat.IsTargetInRangeOf(enemy.combat.meleeAttack2[0]);
        isTargetInMeleeAttack3Range = enemy.combat.IsTargetInRangeOf(enemy.combat.meleeAttack3[0]);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (isGrounded)
            {
                if (!isTargetInDetectionRange)
                {
                    enemy.detection.currentTarget?.entityCombat.targetedBy.Remove(enemy);
                    stateMachine.ChangeState(enemy.idleState);
                }
                else if (enemy.movement.navMeshAgentState != NavMeshAgentState.TraverseAround)
                {
                    meleeAttacks[0] = isTargetInMeleeAttack0Range && enemy.meleeAttack0State.available;
                    meleeAttacks[1] = isTargetInMeleeAttack1Range && enemy.meleeAttack1State.available;
                    meleeAttacks[2] = enemy.detection.currentTarget.entityDetection.currentGroundCollider.Equals(enemy.detection.currentGroundCollider) && enemy.meleeAttack2State.available;
                    meleeAttacks[3] = isTargetInMeleeAttack3Range && enemy.meleeAttack3State.available;

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
                            case 3:
                                stateMachine.ChangeState(enemy.meleeAttack3State); break;
                            default:
                                break;
                        }
                    }
                }
            }

            repositioningTimer.Tick();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            float targetDirection = enemy.detection.currentTarget.entityDetection.currentProjectedPosition.x - enemy.detection.currentProjectedPosition.x > 0 ? 1 : -1;
            enemy.movement.CheckIfShouldFlip(targetDirection);

            if (navMeshAgentState == NavMeshAgentState.Chase)
            {
                currentDestination = enemy.detection.currentTarget.entityDetection.currentProjectedPosition - enemy.orthogonalRigidbody.transform.right * enemy.enemyData.adequateDistance + (Vector3)positionOffset;

                if (enemy.navMeshAgent.enabled)
                {
                    enemy.navMeshAgent.SetDestination(currentDestination);
                    enemy.animator.SetBool("move", enemy.navMeshAgent.remainingDistance > enemy.navMeshAgent.stoppingDistance);
                    enemy.animator.SetBool("idle", enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance);

                    if (!enemy.navMeshAgent.pathPending && enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance && !enemy.navMeshAgent.hasPath)
                    {
                        enemy.navMeshAgent.enabled = false;
                    }
                }
                else
                {
                    if (Vector3.Distance(enemy.detection.currentProjectedPosition, enemy.detection.currentTarget.entityDetection.currentProjectedPosition) > enemy.enemyData.maxDistance)
                    {
                        positionOffset = Random.insideUnitCircle * enemy.enemyData.repositionOffsetDistance;
                        enemy.navMeshAgent.enabled = true;
                    }
                }
            }
            else if (navMeshAgentState == NavMeshAgentState.TraverseAround)
            {
                if (!traverseAroundFlag && !enemy.navMeshAgent.pathPending && enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance && !enemy.navMeshAgent.hasPath)
                {
                    currentDestination = traverseDirection == 1 ? baseDestinationPosition + Vector3.right * enemy.enemyData.adequateDistance + (Vector3)positionOffset : baseDestinationPosition - Vector3.right * enemy.enemyData.adequateDistance;
                    enemy.navMeshAgent.SetDestination(currentDestination);
                    traverseAroundFlag = true;
                }
                else if (traverseAroundFlag && !enemy.navMeshAgent.pathPending && enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance && !enemy.navMeshAgent.hasPath)
                {
                    navMeshAgentState = NavMeshAgentState.Chase;
                }
            }

            enemy.animator.SetBool("moveBack", enemy.navMeshAgent.desiredVelocity.x * targetDirection < 0);
            /*if (navMeshAgentState == NavMeshAgentState.Halt)
            {

            }
            else if (navMeshAgentState == NavMeshAgentState.InDistance)
            {
                if (Vector3.Distance(enemy.detection.currentProjectedPosition, enemy.detection.currentTarget.entityDetection.currentProjectedPosition) < enemy.enemyData.maxDistance)
                {
                    if (Mathf.Abs(enemy.detection.currentProjectedPosition.y - enemy.detection.currentTarget.entityDetection.currentProjectedPosition.y) > enemy.enemyData.repositionOffsetDistance)
                    {

                    }
                    else
                    {

                    }
                }
                else
                {
                    ChangeNavMeshState(NavMeshAgentState.Chase);
                }
            }
            else if (navMeshAgentState == NavMeshAgentState.Chase)
            {
                currentDestination = enemy.detection.currentTarget.entityDetection.currentProjectedPosition - enemy.transform.right * enemy.enemyData.adequateDistance + (Vector3)positionOffset;

                while (enemy.detection.GetPositionGroundCollider(currentDestination) != enemy.detection.currentGroundCollider || Vector3.Distance(enemy.detection.currentProjectedPosition, enemy.detection.currentTarget.entityDetection.currentProjectedPosition) > enemy.enemyData.maxDistance)
                {
                    currentDestination += enemy.transform.right * stepSize;

                    if ((targetDirection > 0 && currentDestination.x > enemy.detection.currentTarget.entityDetection.currentProjectedPosition.x) || (targetDirection < 0 && currentDestination.x < enemy.detection.currentTarget.entityDetection.currentProjectedPosition.x)) break;
                }

                if (!((targetDirection > 0 && currentDestination.x > enemy.detection.currentTarget.entityDetection.currentProjectedPosition.x) || (targetDirection < 0 && currentDestination.x < enemy.detection.currentTarget.entityDetection.currentProjectedPosition.x)))
                {
                    navMeshAgent.SetDestination(currentDestination);
                    recaliberationPosition = enemy.transform.position;
                    Vector3 chaseDirection = navMeshAgent.desiredVelocity == Vector3.zero ? (currentDestination - navMeshAgent.transform.position).normalized : navMeshAgent.velocity.normalized;
                    navMeshAgent.speed = Mathf.Abs(chaseDirection.x) * enemy.enemyData.moveSpeed.x + Mathf.Abs(chaseDirection.y) * enemy.enemyData.moveSpeed.y;
                    enemy.animator.SetBool("moveBack", navMeshAgent.desiredVelocity.x * targetDirection < 0);

                    if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !navMeshAgent.hasPath)
                    {
                        ChangeNavMeshState(NavMeshAgentState.InDistance);
                    }
                }
                else
                {
                    ChangeNavMeshState(NavMeshAgentState.Halt);
                }
            }
            else if (navMeshAgentState == NavMeshAgentState.TraverseAround)
            {
                if (!traverseAroundFlag && !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !navMeshAgent.hasPath)
                {
                    currentDestination = baseDestinationPosition + enemy.transform.right * enemy.enemyData.adequateDistance + (Vector3)positionOffset;

                    while (enemy.detection.GetPositionGroundCollider(currentDestination) != enemy.detection.currentTarget.entityDetection.currentGroundCollider)
                    {
                        currentDestination -= enemy.transform.right * stepSize;
                    }

                    traverseAroundFlag = true;
                }

                navMeshAgent.SetDestination(currentDestination);
                recaliberationPosition = enemy.transform.position;
                Vector3 traverseDirection = navMeshAgent.desiredVelocity == Vector3.zero ? (currentDestination - navMeshAgent.transform.position).normalized : navMeshAgent.velocity.normalized;
                navMeshAgent.speed = Mathf.Abs(traverseDirection.x) * enemy.enemyData.moveSpeed.x + Mathf.Abs(traverseDirection.y) * enemy.enemyData.moveSpeed.y;
                enemy.animator.SetBool("moveBack", navMeshAgent.desiredVelocity.x * targetDirection < 0);

                if (traverseAroundFlag && !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !navMeshAgent.hasPath)
                {
                    ChangeNavMeshState(NavMeshAgentState.InDistance);
                }
            }
            else
            {
                Debug.LogWarning($"Unknown navMeshAgentState of {navMeshAgentState} found in {enemy.name}.");
            }*/
        }
    }
}
