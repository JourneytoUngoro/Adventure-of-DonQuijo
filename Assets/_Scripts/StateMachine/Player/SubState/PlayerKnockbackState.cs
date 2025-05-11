using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockbackState : PlayerState
{
    public Timer knockbackTimer { get; private set; }
    
    // TODO: Make bounce back when hit wall
    // TODO: When posture is maxed, the entity will transit to stunnedState. For the purpose of good game play experience, the stunnedStateRecoveryTimer will be ticked no matter what state the player currently is.
    // TODO: Would it be better to make player uncontrollable during all the time in stunnedState? Or would it be better to recover from the stunnedState right after it gets hit?
    private bool bounceBack;
    private bool shouldTransitToStunnedState;

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

        shouldTransitToStunnedState = player.stats.posture.currentValue == player.stats.posture.maxValue;
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
                    stateMachine.ChangeState(player.idleState);
                }
                else
                {
                    stateMachine.ChangeState(player.inAirState);
                }
            }
        }

        if (!onStateExit)
        {
            if (player.animator.GetBool("airborne"))
            {
                canTransit = isGrounded;
            }
        }
    }

    public void TransitedToStunnedState() => shouldTransitToStunnedState = false;
}
