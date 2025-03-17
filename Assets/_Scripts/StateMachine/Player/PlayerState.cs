using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerState : EntityState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;

    #region Input Variables
    protected int inputX;
    protected int inputY;

    protected bool jumpInputHolding;
    protected bool jumpInputPressed;

    protected bool dodgeInputPressed;
    #endregion

    protected Vector2 facingObstacleHeight;
    protected Vector2 colliderInitialOffset;
    protected Vector2 stateEnterInitialPosition;

    public PlayerState(Player player, string animBoolName) : base(player, animBoolName)
    {
        this.player = player;
        stateMachine = player.playerStateMachine;
        colliderInitialOffset = player.entityCollider.offset;
    }

    public override void Enter()
    {
        base.Enter();

        SetInputVariables();
        stateEnterInitialPosition = player.transform.position;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        SetInputVariables();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    protected override void SetMovementVariables()
    {
        base.SetMovementVariables();

        facingObstacleHeight = player.entityDetection.facingObstacleHeight;
    }

    protected void SetInputVariables()
    {
        inputX = Manager.Instance.inputHandler.normInputX;
        inputY = Manager.Instance.inputHandler.normInputY;
        jumpInputHolding = Manager.Instance.inputHandler.jumpInputHolding;
        jumpInputPressed = Manager.Instance.inputHandler.jumpInputPressed;
        dodgeInputPressed = Manager.Instance.inputHandler.dodgeInputPressed;
    }

    protected override void TickPublicTimers()
    {
        base.TickPublicTimers();

        player.dodgeState.dodgeCoolDownTimer.Tick();
    }

    protected virtual void FixColliderPositionY()
    {

    }
}
