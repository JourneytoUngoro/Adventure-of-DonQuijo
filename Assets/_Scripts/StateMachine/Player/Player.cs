using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    public PlayerStateMachine playerStateMachine;

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerInAirState inAirState { get; private set; }
    public PlayerDodgeState dodgeState { get; private set; }

    #region Player Componenets
    public PlayerMovement movement { get; private set; }
    #endregion

    protected override void Start()
    {
        base.Start();

        movement = entityMovement as PlayerMovement;

        playerStateMachine = new PlayerStateMachine();
        entityStateMachine = playerStateMachine;

        idleState = new PlayerIdleState(this, "idle");
        moveState = new PlayerMoveState(this, "move");
        jumpState = new PlayerJumpState(this, "inAir");
        inAirState = new PlayerInAirState(this, "inAir");
        dodgeState = new PlayerDodgeState(this, "dodge");

        playerStateMachine.Initialize(idleState);
    }
}
