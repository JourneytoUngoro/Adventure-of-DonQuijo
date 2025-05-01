using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : EntityState
{
    public Player player { get; private set; }
    protected PlayerData playerData;
    protected PlayerStateMachine stateMachine;

    #region Input Variables
    protected int inputX;
    protected int inputY;

    protected bool jumpInputHolding;
    protected bool jumpInputPressed;
    protected bool blockParryInputHolding;
    protected bool blockParryInputPressed;
    protected bool attackInputPressed;
    protected bool dodgeInputPressed;
    #endregion

    protected Vector2 stateEnterInitialPosition;

    public PlayerState(Player player, string animBoolName) : base(player, animBoolName)
    {
        this.player = player;
        playerData = player.playerData;
        stateMachine = player.playerStateMachine;
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
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        SetInputVariables();
    }

    protected void SetInputVariables()
    {
        inputX = Manager.Instance.inputHandler.normInputX;
        inputY = Manager.Instance.inputHandler.normInputY;
        jumpInputHolding = Manager.Instance.inputHandler.jumpInputHolding;
        jumpInputPressed = Manager.Instance.inputHandler.jumpInputPressed;
        blockParryInputHolding = Manager.Instance.inputHandler.blockParryInputHolding;
        blockParryInputPressed = Manager.Instance.inputHandler.blockParryInputPressed;
        attackInputPressed = Manager.Instance.inputHandler.attackInputPressed;
        dodgeInputPressed = Manager.Instance.inputHandler.dodgeInputPressed;
    }

    protected override void TickPublicTimers()
    {
        base.TickPublicTimers();

        foreach (PlayerAbilityState abilityState in player.abilityStates)
        {
            abilityState.abilityCoolDownTimer.Tick();
        }
        player.moveState.dashInputXBufferTimer.Tick();
        player.moveState.dashInputYBufferTimer.Tick();
        player.attackState.attackComboTimer.Tick();
    }
}
