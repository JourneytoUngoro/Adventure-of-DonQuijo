using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMeleeAttack2State : EnemyAbilityState
{
    private Timer makeDistanceTimer;
    private bool makeDistanceTimeElapsed;
    private bool attackConfirmed;
    private bool isTargetInMeleeAttack2Range;
    private Vector3 attackPosition;

    public EnemyMeleeAttack2State(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        available = true;
        abilityCoolDownTimer.ChangeDuration(enemyData.meleeAttack2CoolDown);
        makeDistanceTimer = new Timer(enemyData.meleeAttack2MakeDistanceTime);
        makeDistanceTimer.timerAction += () => { makeDistanceTimeElapsed = true; };
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        enemy.combat.DoAttack(enemy.combat.meleeAttack2[index]);
        enemy.combat.damagedTargets.Clear();
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
        attackConfirmed = false;
        enemy.movement.SetVelocityZero();
        enemy.animator.SetInteger("typeIndex", 2);
        makeDistanceTimeElapsed = false;
        makeDistanceTimer.StartSingleUseTimer();

        enemy.navMeshAgent.enabled = true;

        // Set First Destination
        // float targetDirection = enemy.detection.currentTarget.entityDetection.currentProjectedPosition.x - enemy.detection.currentProjectedPosition.x > 0 ? 1 : -1;
        Vector3 currentDestination = enemy.detection.currentTarget.entityDetection.currentProjectedPosition - enemy.transform.right * enemyData.meleeAttack2DashDistance;

        /*while (enemy.detection.GetPositionGroundCollider(currentDestination) != enemy.detection.currentGroundCollider)
        {
            currentDestination += enemy.transform.right * enemy.enemyData.stepSize;

            if ((targetDirection > 0 && currentDestination.x > enemy.detection.currentTarget.entityDetection.currentProjectedPosition.x) || (targetDirection < 0 && currentDestination.x < enemy.detection.currentTarget.entityDetection.currentProjectedPosition.x)) break;
        }*/

        enemy.navMeshAgent.SetDestination(currentDestination);
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isTargetInMeleeAttack2Range = enemy.combat.IsTargetInRangeOf(enemy.combat.meleeAttack2[0]);
    }

    public override void Exit()
    {
        enemy.animator.SetBool("meleeAttack", false);
        enemy.animator.SetBool("move", false);
        enemy.animator.SetBool("dash", false);
        
        base.Exit();

        enemy.navMeshAgent.enabled = false;
        abilityCoolDownTimer.StartSingleUseTimer();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            if (enemy.navMeshAgent.enabled)
            {
                float targetDirection = enemy.detection.currentTarget.entityDetection.currentProjectedPosition.x - enemy.detection.currentProjectedPosition.x > 0 ? 1 : -1;
                enemy.movement.CheckIfShouldFlip(targetDirection);

                if (!attackConfirmed && !enemy.navMeshAgent.pathPending && enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance && !enemy.navMeshAgent.hasPath)
                {
                    attackConfirmed = true;
                    enemy.navMeshAgent.SetDestination(enemy.detection.currentTarget.entityDetection.currentProjectedPosition);
                }

                if (attackConfirmed)
                {
                    enemy.animator.SetBool("dash", true);

                    if (isTargetInMeleeAttack2Range)
                    {
                        enemy.navMeshAgent.enabled = false;
                        enemy.animator.SetBool("move", false);
                        enemy.animator.SetBool("meleeAttack", true);
                    }
                }
                else if (isTargetInMeleeAttack2Range && makeDistanceTimeElapsed)
                {
                    enemy.navMeshAgent.enabled = false;
                    enemy.animator.SetBool("move", false);
                    enemy.animator.SetBool("meleeAttack", true);
                }

                enemy.animator.SetBool("moveBack", enemy.navMeshAgent.desiredVelocity.x * targetDirection < 0);
            }
        }
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
                        stateMachine.ChangeState(enemy.targetInDetectionRangeState);
                    }
                    else
                    {
                        stateMachine.ChangeState(enemy.idleState);
                    }
                }
                else
                {
                    // stateMachine.ChangeState(enemy.inAirState);
                }
            }

            makeDistanceTimer.Tick();
        }
    }
}
