using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockbackState : PlayerState
{
    public Timer knockbackTimer { get; private set; }
    
    // TODO: Make bounce back when hit wall
    // TODO: When posture is maxed, the entity will transit to stunnedState. For the purpose of good game play experience, the stunnedStateRecoveryTimer will be ticked no matter what state the player currently is.
    // TODO: Would it be better to make player uncontrollable during all the time in stunnedState? Or would it be better to recover from the stunnedState right after it gets hit?
    private bool shouldTransitToStunnedState;
    private bool shouldTransitToDeadState;
    private Vector3 knockbackVelocityBeforeCollision;

    public PlayerKnockbackState(Player player, string animBoolName) : base(player, animBoolName)
    {
        knockbackTimer = new Timer(0.0f);
        knockbackTimer.timerAction += () => { canTransit = true; };
    }

    public override void Enter()
    {
        base.Enter();

        canTransit = false;

        player.animator.SetInteger("typeIndex", UtilityFunctions.RandomInteger(3));

        if (isGrounded)
        {
            knockbackTimer.StartSingleUseTimer();
        }
        else
        {
            player.orthogonalRigidbody.gravityScale = playerData.gravityScale;
        }

        shouldTransitToStunnedState = player.stats.posture.currentValue == player.stats.posture.minValue;
        shouldTransitToDeadState = player.stats.health.currentValue == player.stats.health.minValue;
        Manager.Instance.soundFXManager.PlaySoundFXClip(Manager.Instance.soundFXManager.playerHitSoundFX, player.transform);
    }

    public override void Exit()
    {
        base.Exit();

        // TODO: Invinsible after fall
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            player.animator.SetBool("inAir", !isGrounded);
            player.animator.SetBool("velocityXbiggerthanZ", Mathf.Abs(currentVelocity.x) > Mathf.Abs(currentVelocity.z));
            player.animator.SetFloat("velocityZ", currentVelocity.z);
            player.knockbackState.knockbackTimer.Tick(isGrounded, true);
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
                        stateMachine.ChangeState(player.deadState);
                    }
                    else if (shouldTransitToStunnedState)
                    {
                        stateMachine.ChangeState(player.stunnedState);
                    }
                    else
                    {
                        stateMachine.ChangeState(player.idleState);
                    }
                }
                /*else
                {
                    stateMachine.ChangeState(player.inAirState);
                }*/
            }
        }

        if (!onStateExit)
        {
            if (player.animator.GetBool("airborne"))
            {
                if (!player.movement.onContact) knockbackVelocityBeforeCollision = currentVelocity;
                canTransit = isGrounded && knockbackVelocityBeforeCollision.magnitude < player.playerData.knockbackReboundThresholdSpeed;

                if (!canTransit)
                {
                    float velocityXAbsolute = Mathf.Abs(knockbackVelocityBeforeCollision.x);
                    float velocityZAbsolute = Mathf.Abs(knockbackVelocityBeforeCollision.z);
                    float velocityAngle = Mathf.Atan2(Mathf.Abs(knockbackVelocityBeforeCollision.z), Mathf.Abs(knockbackVelocityBeforeCollision.x)) * Mathf.Rad2Deg;
                    
                    if (isGrounded)
                    {
                        player.movement.SetVelocityX(knockbackVelocityBeforeCollision.x * UtilityFunctions.DeviationFloat(player.playerData.decelerationRatio, 0.1f));
                        player.movement.SetVelocityY(knockbackVelocityBeforeCollision.y * UtilityFunctions.DeviationFloat(player.playerData.decelerationRatio, 0.1f));
                        player.movement.SetVelocityZ(knockbackVelocityBeforeCollision.z * -UtilityFunctions.DeviationFloat(player.playerData.decelerationRatio, 0.1f));
                    }
                    // TODO: Currently does not support Y-Axis Knockback
                    else if (player.movement.onContact)
                    {
                        player.movement.SetVelocityY(knockbackVelocityBeforeCollision.y);

                        if (knockbackVelocityBeforeCollision.x > 0 && player.detection.detectingHorizontalObstacle.first)
                        {
                            /*if (knockbackVelocityBeforeCollision.magnitude < player.playerData.knockbackReboundThresholdSpeed)
                            {
                                if (velocityXAbsolute > velocityZAbsolute)
                                {
                                    player.movement.SetVelocityX(knockbackVelocityBeforeCollision.x * player.playerData.decelerationRatio);
                                    player.movement.SetVelocityZ(knockbackVelocityBeforeCollision.z * -player.playerData.decelerationRatio);
                                }
                                else
                                {

                                }
                            }*/
                        }
                        else if (knockbackVelocityBeforeCollision.x < 0 && player.detection.detectingHorizontalObstacle.second)
                        {
                            if (knockbackVelocityBeforeCollision.magnitude < player.playerData.knockbackReboundThresholdSpeed)
                            {
                                if (knockbackVelocityBeforeCollision.z > 0)
                                {
                                    player.movement.SetVelocityX(knockbackVelocityBeforeCollision.x * -UtilityFunctions.DeviationFloat(player.playerData.decelerationRatio, 0.1f));
                                    player.movement.SetVelocityZ(knockbackVelocityBeforeCollision.z * UtilityFunctions.DeviationFloat(player.playerData.decelerationRatio, 0.1f));
                                }
                                else
                                {
                                    player.movement.SetVelocityX(knockbackVelocityBeforeCollision.x * -UtilityFunctions.DeviationFloat(player.playerData.decelerationRatio, 0.1f));
                                    player.movement.SetVelocityZ(knockbackVelocityBeforeCollision.z);
                                }
                            }
                            else
                            {
                                if (knockbackVelocityBeforeCollision.z > 0)
                                {
                                    if (velocityAngle < player.playerData.wallKnockbackReboundThresholdAngle)
                                    {
                                        float reboundRadian = UtilityFunctions.DeviationFloat(player.playerData.wallKnockbackReboundThresholdAngle, 10.0f) * Mathf.Deg2Rad;
                                        Vector2 reboundVector = new Vector2(Mathf.Abs(Mathf.Cos(reboundRadian)), Mathf.Abs(Mathf.Sin(reboundRadian)));
                                        player.movement.SetVelocityX(knockbackVelocityBeforeCollision.magnitude * reboundVector.x * UtilityFunctions.DeviationFloat(player.playerData.decelerationRatio, 0.1f));
                                        player.movement.SetVelocityZ(knockbackVelocityBeforeCollision.magnitude * reboundVector.y * UtilityFunctions.DeviationFloat(player.playerData.decelerationRatio, 0.1f));
                                    }
                                    else
                                    {
                                        player.movement.SetVelocityX(knockbackVelocityBeforeCollision.x * -player.playerData.decelerationRatio);
                                        player.movement.SetVelocityZ(knockbackVelocityBeforeCollision.z * (1.0f - player.playerData.decelerationRatio));
                                    }
                                }
                                else
                                {
                                    if (velocityAngle < player.playerData.wallKnockbackReboundThresholdAngle)
                                    {
                                        float reboundRadian = UtilityFunctions.DeviationFloat(player.playerData.wallKnockbackReboundThresholdAngle, 10.0f) * Mathf.Deg2Rad;
                                        Vector2 reboundVector = new Vector2(Mathf.Abs(Mathf.Cos(reboundRadian)), Mathf.Abs(Mathf.Sin(reboundRadian)));
                                        player.movement.SetVelocityX(knockbackVelocityBeforeCollision.magnitude * reboundVector.x * UtilityFunctions.DeviationFloat(player.playerData.decelerationRatio, 0.1f));
                                        player.movement.SetVelocityZ(knockbackVelocityBeforeCollision.magnitude * reboundVector.y * UtilityFunctions.DeviationFloat(player.playerData.decelerationRatio, 0.1f));
                                    }
                                    else
                                    {
                                        player.movement.SetVelocityX(knockbackVelocityBeforeCollision.x * -UtilityFunctions.DeviationFloat(player.playerData.decelerationRatio, 0.1f));
                                        player.movement.SetVelocityZ(knockbackVelocityBeforeCollision.z);
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
