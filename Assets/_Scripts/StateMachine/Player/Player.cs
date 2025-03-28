using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Player : Entity
{
    #region State Variables
    public PlayerStateMachine playerStateMachine;

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerInAirState inAirState { get; private set; }
    public PlayerDodgeState dodgeState { get; private set; }
    public PlayerStunnedState stunnedState { get; private set; }
    public PlayerLandingState landingState { get; private set; }
    public PlayerKnockbackState knockbackState { get; private set; }
    public PlayerDeadState deadState { get; private set; }

    public List<PlayerAbilityState> abilityStates { get; private set; }
    #endregion

    #region Player Componenets
    public PlayerMovement movement { get; private set; }
    public PlayerDetection detection { get; private set; }
    public PlayerStats stats { get; private set; }
    public PlayerData playerData { get; private set; }
    #endregion

    protected override void Start()
    {
        base.Start();

        movement = entityMovement as PlayerMovement;
        detection = entityDetection as PlayerDetection;
        stats = entityStats as PlayerStats;
        playerData = entityData as PlayerData;

        playerStateMachine = new PlayerStateMachine();
        entityStateMachine = playerStateMachine;

        idleState = new PlayerIdleState(this, "idle");
        moveState = new PlayerMoveState(this, "move");
        jumpState = new PlayerJumpState(this, "inAir");
        wallJumpState = new PlayerWallJumpState(this, "wallJump");
        inAirState = new PlayerInAirState(this, "inAir");
        dodgeState = new PlayerDodgeState(this, "dodge");
        stunnedState = new PlayerStunnedState(this, "stunned");
        landingState = new PlayerLandingState(this, "landing");
        knockbackState = new PlayerKnockbackState(this, "knockback");
        deadState = new PlayerDeadState(this, "dead");

        abilityStates = new List<PlayerAbilityState>();
        IEnumerable<PropertyInfo> abilityStateProperties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.PropertyType.IsSubclassOf(typeof(PlayerAbilityState)));

        foreach (PropertyInfo property in abilityStateProperties)
        {
            PlayerAbilityState abilityState = property.GetValue(this) as PlayerAbilityState;
            abilityStates.Add(abilityState);
        }

        playerStateMachine.Initialize(idleState);
    }
}
