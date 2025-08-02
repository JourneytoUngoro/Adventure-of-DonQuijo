using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Entity
{
    public static Player Instance { get; private set; }

    #region State Variables
    public PlayerStateMachine playerStateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerInAirState inAirState { get; private set; }
    public PlayerDodgeState dodgeState { get; private set; }
    public PlayerStunnedState stunnedState { get; private set; }
    public PlayerLandingState landingState { get; private set; }
    public PlayerKnockbackState knockbackState { get; private set; }
    public PlayerMeleeAttack0State meleeAttack0State { get; private set; }
    public PlayerMeleeAttack1State meleeAttack1State { get; private set; }
    public PlayerMeleeAttack2State meleeAttack2State { get; private set; }
    public PlayerStrongAttackState strongAttackState { get; private set; }
    public PlayerDashAttackState dashAttackState { get; private set; }
    public PlayerStrongDashAttackState strongDashAttackState { get; private set; }
    public PlayerBlockParryState blockParryState { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    public PlayerCureState cureState { get; private set; }
    public PlayerWhirlwindState whirlwindState { get; private set; }
    public PlayerRangedAttackState rangedAttackState { get; private set; }
    public PlayerChargeAttackState chargeAttackState { get; private set; }


    public List<PlayerAbilityState> abilityStates { get; private set; }
    #endregion

    #region Player Componenets
    public PlayerMovement movement { get; private set; }
    public PlayerDetection detection { get; private set; }
    public PlayerStats stats { get; private set; }
    public PlayerCombat combat { get; private set; }
    public PlayerData playerData { get; private set; }
    #endregion

    #region Other variables
    public List<InteractBase> interactableGameObjects { get; private set; } = new List<InteractBase>();
    #endregion

    protected override void Awake()
    {
        base.Awake();

        #region Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(false);
        #endregion
    }

    protected override void Start()
    {
        base.Start();

        movement = entityMovement as PlayerMovement;
        detection = entityDetection as PlayerDetection;
        stats = entityStats as PlayerStats;
        combat = entityCombat as PlayerCombat;
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
        meleeAttack0State = new PlayerMeleeAttack0State(this, "meleeAttack");
        meleeAttack1State = new PlayerMeleeAttack1State(this, "meleeAttack");
        meleeAttack2State = new PlayerMeleeAttack2State(this, "meleeAttack");
        strongAttackState = new PlayerStrongAttackState(this, "strongAttack");
        dashAttackState = new PlayerDashAttackState(this, "dashAttack");
        strongDashAttackState = new PlayerStrongDashAttackState(this, "strongDashAttack");
        blockParryState = new PlayerBlockParryState(this, "blockParry");
        deadState = new PlayerDeadState(this, "dead");
        cureState = new PlayerCureState(this, "cure");

        whirlwindState = new PlayerWhirlwindState(this, "whirlwind"); // added variables
        rangedAttackState = new PlayerRangedAttackState(this, "rangedAttack");
        chargeAttackState = new PlayerChargeAttackState(this, "chargeAttack");

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

    private void OnEnable()
    {
        Manager.Instance.inputHandler.playerInput.currentActionMap.Enable();
    }

    private void OnDisable()
    {
        Manager.Instance.inputHandler.playerInput.currentActionMap.Disable();
    }

    public void Revive(bool resetPosition)
    {
        if (resetPosition)
        {
            detection.SetPosition(new Vector3(-600.0f, -100.0f, 0.0f));
        }

        isDead = false;
        animator.SetBool("dead", false);
        gameObject.tag = "Idle";
        stats.health.SetCurrentValue(stats.health.maxValue);
        stats.posture.SetCurrentValue(stats.posture.maxValue);
        stats.posture.ControlRecoveryTimer(TimerControl.Start);
        playerStateMachine.Initialize(idleState);
    }
}
