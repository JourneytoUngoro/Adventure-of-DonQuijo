using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EntityState
{
    #region Components
    public Entity entity { get; private set; }
    protected EntityStateMachine entityStateMachine;
    protected string animBoolName;
    #endregion

    #region State Variables
    protected bool onStateExit;
    protected bool isAnimationStarted;
    protected bool isAnimationActionTriggered;
    protected bool isAnimationFinished;
    #endregion

    #region Physics Variables
    protected Vector3 currentScreenPosition;
    protected Vector3 currentProjectedPosition;
    protected Vector3 currentSpacePosition;
    protected Vector2 currentVelocity;
    protected float currentEntityHeight;
    protected float currentGroundHeight;
    protected int facingDirection;
    protected bool isGrounded;
    #endregion

    #region Other Variables
    // protected Timer afterImageTimer;

    protected Vector2 v2WorkSpace;
    protected Vector3 v3WorkSpace;

    protected float epsilon = 0.001f;
    #endregion

    public float startTime { get; protected set; }

    public EntityState(Entity entity, string animBoolName)
    {
        this.entity = entity;
        this.entityStateMachine = entity.entityStateMachine;
        this.animBoolName = animBoolName;
        entity.entityMovement.synchronizeValues += SetMovementVariables;
        // afterImageTimer = new Timer(0.1f);
        // afterImageTimer.StartMultiUseTimer();
    }

    public virtual void AnimationStartTrigger(int index)
    {
        isAnimationStarted = true;
        isAnimationFinished = false;
    }

    public virtual void AnimationFinishTrigger(int index)
    {
        isAnimationFinished = true;
    }

    public virtual void AnimationActionTrigger(int index)
    {
        isAnimationActionTriggered = true;
    }

    public virtual void DoChecks()
    {
        isGrounded = entity.entityDetection.isGrounded();
    }

    public virtual void Enter()
    {
        startTime = Time.time;
        onStateExit = false;
        isAnimationStarted = false;
        isAnimationActionTriggered = false;
        isAnimationFinished = false;

        entity.animator.SetBool(animBoolName, true);
        SetMovementVariables();
        DoChecks();
    }

    public virtual void Exit()
    {
        onStateExit = true;
        // entity.entityCombat.ChangeStanceLevel(Intensity.Ðñ);
        entity.animator.SetBool(animBoolName, false);
    }

    public virtual void LogicUpdate()
    {
        TickPublicTimers();
        SetMovementVariables();
        entity.shadow.transform.position = currentProjectedPosition + Vector3.up * currentProjectedPosition.z;
    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
        SetMovementVariables();
        entity.shadow.transform.position = currentProjectedPosition + Vector3.up * currentProjectedPosition.z;
    }

    public virtual void LateLogicUpdate()
    {

    }

    protected virtual void SetMovementVariables()
    {
        currentVelocity = entity.rigidBody.velocity;
        currentScreenPosition = entity.entityDetection.currentScreenPosition;
        currentProjectedPosition = entity.entityDetection.currentProjectedPosition;
        currentSpacePosition = entity.entityDetection.currentSpacePosition;
        currentEntityHeight = entity.entityDetection.currentEntityHeight;
        currentGroundHeight = entity.entityDetection.currentGroundHeight;
        facingDirection = entity.entityMovement.facingDirection;
    }

    /// <summary>
    /// This function is used to tick public timers that should run outside the state. For example, is the player dodges, the dodge cool down timer will be initiated in DodgeState, but dodge cool down timer should run whether player's current state is DodgeState or not.
    /// Make the Timer class 'public' and place Tick() method inside the TickPublicTimers() Function.
    /// </summary>
    protected virtual void TickPublicTimers()
    {
        
    }
}
