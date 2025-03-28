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
    protected Vector3 currentVelocity;
    protected float currentEntityHeight;
    protected float currentGroundHeight;
    protected int facingDirection;
    protected bool isGrounded;
    #endregion

    #region Other Variables
    // protected Timer afterImageTimer;
    protected bool canTransit; // This variable is to keep entity's state fixed for specific amount of time. For example, when the entity gets hit, it needs to stay in GotHitState for a few seconds before transiting to other states.
    protected Vector3 workSpace;
    protected float epsilon = 0.001f;
    private Vector3 shadowOffset;
    #endregion

    public float startTime { get; protected set; }

    public EntityState(Entity entity, string animBoolName)
    {
        this.entity = entity;
        this.entityStateMachine = entity.entityStateMachine;
        this.animBoolName = animBoolName;
        entity.entityMovement.synchronizeValues += SetMovementVariables;
        shadowOffset = entity.shadow.localPosition;
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
        canTransit = true;
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
    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
        SetMovementVariables();
        workSpace.Set(shadowOffset.x, currentGroundHeight + shadowOffset.y, currentGroundHeight);
        entity.shadow.localPosition = workSpace;
    }

    public virtual void LateLogicUpdate()
    {

    }

    protected virtual void SetMovementVariables()
    {
        workSpace.Set(entity.rigidbody.velocity.x, entity.rigidbody.velocity.y, entity.orthogonalRigidbody.velocity);
        currentVelocity = workSpace;
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
