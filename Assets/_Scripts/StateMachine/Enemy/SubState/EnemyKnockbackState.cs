using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKnockbackState : EnemyState
{
    public Timer knockbackTimer { get; private set; }

    // TODO: When posture is maxed, the entity will transit to stunnedState. For the purpose of good game play experience, the stunnedStateRecoveryTimer will be ticked no matter what state the enemy currently is.
    // TODO: Would it be better to make enemy uncontrollable during all the time in stunnedState? Or would it be better to recover from the stunnedState right after it gets hit?
    private bool shouldTransitToStunnedState;
    private bool shouldTransitToDeadState;
    private Vector3 knockbackVelocityBeforeCollision;

    public EnemyKnockbackState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        knockbackTimer = new Timer(0.0f);
        knockbackTimer.timerAction += () => { canTransit = true; };
    }

    public override void Enter()
    {
        base.Enter();

        canTransit = false;

        enemy.animator.SetInteger("typeIndex", UtilityFunctions.RandomInteger(3));

        if (isGrounded)
        {
            knockbackTimer.StartSingleUseTimer();
        }
        else
        {
            enemy.orthogonalRigidbody.gravityScale = enemyData.gravityScale;
        }

        shouldTransitToStunnedState = enemy.stats.posture.currentValue == enemy.stats.posture.minValue;
        shouldTransitToDeadState = enemy.stats.health.currentValue == enemy.stats.health.minValue;
        enemy.stats.posture.ControlRecoveryTimer(TimerControl.Stop);

        Manager.Instance.soundManager.PlaySoundFXClip("", enemy.transform);
    }

    public override void Exit()
    {
        base.Exit();

        // TODO: Invinsible after fall
        enemy.movement.SetVelocityZero();
        enemy.movement.StopVelocityChangeOverTime();
        enemy.stats.posture.ControlRecoveryTimer(TimerControl.Start);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            enemy.animator.SetBool("inAir", !isGrounded);
            enemy.animator.SetBool("velocityXbiggerthanZ", Mathf.Abs(currentVelocity.x) > Mathf.Abs(currentVelocity.z));
            enemy.animator.SetFloat("velocityZ", currentVelocity.z);
            enemy.knockbackState.knockbackTimer.Tick(isGrounded, true);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            if (canTransit)
            {
                if (isGrounded)
                {
                    if (shouldTransitToDeadState)
                    {
                        stateMachine.ChangeState(enemy.deadState);
                    }
                    else if (shouldTransitToStunnedState)
                    {
                        stateMachine.ChangeState(enemy.stunnedState);
                    }
                    else
                    {
                        if (isTargetInDetectionRange)
                        {
                            stateMachine.ChangeState(enemy.targetInDetectionRangeState);
                        }
                        else
                        {
                            /*if ((enemy.detection.currentTarget.entityDetection.currentProjectedPosition.x - enemy.detection.currentProjectedPosition.x) * facingDirection < 0)
                            {
                                enemy.movement.Flip();
                            }*/

                            stateMachine.ChangeState(enemy.idleState);
                        }
                    }
                }
                /*else
                {
                    stateMachine.ChangeState(enemy.inAirState);
                }*/
            }
        }

        if (!onStateExit)
        {
            if (enemy.animator.GetBool("airborne"))
            {
                if (!enemy.movement.onContact) knockbackVelocityBeforeCollision = currentVelocity;
                canTransit = isGrounded && knockbackVelocityBeforeCollision.magnitude < enemy.enemyData.knockbackReboundThresholdSpeed;

                if (!canTransit)
                {
                    float velocityXAbsolute = Mathf.Abs(knockbackVelocityBeforeCollision.x);
                    float velocityZAbsolute = Mathf.Abs(knockbackVelocityBeforeCollision.z);
                    float velocityAngle = Mathf.Atan2(Mathf.Abs(knockbackVelocityBeforeCollision.z), Mathf.Abs(knockbackVelocityBeforeCollision.x)) * Mathf.Rad2Deg;

                    if (isGrounded)
                    {
                        enemy.movement.SetVelocityX(knockbackVelocityBeforeCollision.x * UtilityFunctions.DeviationFloat(enemy.enemyData.decelerationRatio, 0.1f));
                        enemy.movement.SetVelocityY(knockbackVelocityBeforeCollision.y * UtilityFunctions.DeviationFloat(enemy.enemyData.decelerationRatio, 0.1f));
                        enemy.movement.SetVelocityZ(knockbackVelocityBeforeCollision.z * -UtilityFunctions.DeviationFloat(enemy.enemyData.decelerationRatio, 0.1f));
                    }

                    // TODO: Currently does not support Y-Axis Knockback
                    else if (enemy.movement.onContact)
                    {
                        enemy.movement.SetVelocityY(knockbackVelocityBeforeCollision.y);

                        if (knockbackVelocityBeforeCollision.x > 0 && enemy.detection.detectingHorizontalObstacle.first)
                        {
                            /*if (knockbackVelocityBeforeCollision.magnitude < enemy.enemyData.knockbackReboundThresholdSpeed)
                            {
                                if (velocityXAbsolute > velocityZAbsolute)
                                {
                                    enemy.movement.SetVelocityX(knockbackVelocityBeforeCollision.x * enemy.enemyData.decelerationRatio);
                                    enemy.movement.SetVelocityZ(knockbackVelocityBeforeCollision.z * -enemy.enemyData.decelerationRatio);
                                }
                                else
                                {

                                }
                            }*/
                        }
                        else if (knockbackVelocityBeforeCollision.x < 0 && enemy.detection.detectingHorizontalObstacle.second)
                        {
                            if (knockbackVelocityBeforeCollision.magnitude < enemy.enemyData.knockbackReboundThresholdSpeed)
                            {
                                if (knockbackVelocityBeforeCollision.z > 0)
                                {
                                    enemy.movement.SetVelocityX(knockbackVelocityBeforeCollision.x * -UtilityFunctions.DeviationFloat(enemy.enemyData.decelerationRatio, 0.1f));
                                    enemy.movement.SetVelocityZ(knockbackVelocityBeforeCollision.z * UtilityFunctions.DeviationFloat(enemy.enemyData.decelerationRatio, 0.1f));
                                }
                                else
                                {
                                    enemy.movement.SetVelocityX(knockbackVelocityBeforeCollision.x * -UtilityFunctions.DeviationFloat(enemy.enemyData.decelerationRatio, 0.1f));
                                    enemy.movement.SetVelocityZ(knockbackVelocityBeforeCollision.z);
                                }
                            }
                            else
                            {
                                if (knockbackVelocityBeforeCollision.z > 0)
                                {
                                    if (velocityAngle < enemy.enemyData.wallKnockbackReboundThresholdAngle)
                                    {
                                        float reboundRadian = UtilityFunctions.DeviationFloat(enemy.enemyData.wallKnockbackReboundThresholdAngle, 10.0f) * Mathf.Deg2Rad;
                                        Vector2 reboundVector = new Vector2(Mathf.Abs(Mathf.Cos(reboundRadian)), Mathf.Abs(Mathf.Sin(reboundRadian)));
                                        enemy.movement.SetVelocityX(knockbackVelocityBeforeCollision.magnitude * reboundVector.x * UtilityFunctions.DeviationFloat(enemy.enemyData.decelerationRatio, 0.1f));
                                        enemy.movement.SetVelocityZ(knockbackVelocityBeforeCollision.magnitude * reboundVector.y * UtilityFunctions.DeviationFloat(enemy.enemyData.decelerationRatio, 0.1f));
                                    }
                                    else
                                    {
                                        enemy.movement.SetVelocityX(knockbackVelocityBeforeCollision.x * -enemy.enemyData.decelerationRatio);
                                        enemy.movement.SetVelocityZ(knockbackVelocityBeforeCollision.z * (1.0f - enemy.enemyData.decelerationRatio));
                                    }
                                }
                                else
                                {
                                    if (velocityAngle < enemy.enemyData.wallKnockbackReboundThresholdAngle)
                                    {
                                        float reboundRadian = UtilityFunctions.DeviationFloat(enemy.enemyData.wallKnockbackReboundThresholdAngle, 10.0f) * Mathf.Deg2Rad;
                                        Vector2 reboundVector = new Vector2(Mathf.Abs(Mathf.Cos(reboundRadian)), Mathf.Abs(Mathf.Sin(reboundRadian)));
                                        enemy.movement.SetVelocityX(knockbackVelocityBeforeCollision.magnitude * reboundVector.x * UtilityFunctions.DeviationFloat(enemy.enemyData.decelerationRatio, 0.1f));
                                        enemy.movement.SetVelocityZ(knockbackVelocityBeforeCollision.magnitude * reboundVector.y * UtilityFunctions.DeviationFloat(enemy.enemyData.decelerationRatio, 0.1f));
                                    }
                                    else
                                    {
                                        enemy.movement.SetVelocityX(knockbackVelocityBeforeCollision.x * -UtilityFunctions.DeviationFloat(enemy.enemyData.decelerationRatio, 0.1f));
                                        enemy.movement.SetVelocityZ(knockbackVelocityBeforeCollision.z);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void TransitedToStunnedState() => shouldTransitToStunnedState = false;
}
