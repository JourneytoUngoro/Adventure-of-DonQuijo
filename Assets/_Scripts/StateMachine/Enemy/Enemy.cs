using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Entity
{
    #region State Variables
    public EnemyStateMachine enemyStateMachine { get; private set; }

    public EnemyIdleState idleState { get; private set; }
    public EnemyKnockbackState knockbackState { get; private set; }
    public EnemyStunnedState stunnedState { get; private set; }
    public EnemyTargetInDetectionRangeState targetInDetectionRangeState { get; private set; }
    public EnemyMeleeAttack0State meleeAttack0State { get; private set; }
    public EnemyMeleeAttack1State meleeAttack1State { get; private set; }
    public EnemyMeleeAttack2State meleeAttack2State { get; private set; }
    public EnemyDodgeAttackState dodgeAttackState { get; private set; }
    public EnemyDashAttackState dashAttackState { get; private set; }
    public EnemyWideAttackState wideAttackState { get; private set; }
    // public EnemyRangedAttckState rangedAttckState { get; private set; }
    public EnemyBlockParryState blockParryState { get; private set; }
    public EnemyDeadState deadState { get; private set; }
    public List<EnemyAbilityState> abilityStates { get; protected set; }
    #endregion

    #region Enemy Components
    public EnemyMovement movement { get; private set; }
    public EnemyDetection detection { get; private set; }
    public EnemyStats stats { get; private set; }
    public EnemyCombat combat { get; private set; }
    public EnemyData enemyData { get; private set; }
    public NavMeshAgent navMeshAgent { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }

    protected override void Start()
    {
        base.Start();
        
        movement = entityMovement as EnemyMovement;
        detection = entityDetection as EnemyDetection;
        combat = entityCombat as EnemyCombat;
        stats = entityStats as EnemyStats;
        enemyData = entityData as EnemyData;

        enemyStateMachine = new EnemyStateMachine();
        entityStateMachine = enemyStateMachine;

        idleState = new EnemyIdleState(this, "idle");
        stunnedState = new EnemyStunnedState(this, "stunned");
        knockbackState = new EnemyKnockbackState(this, "knockback");
        meleeAttack0State = new EnemyMeleeAttack0State(this, "meleeAttack0");
        meleeAttack1State = new EnemyMeleeAttack1State(this, "meleeAttack1");
        meleeAttack2State = new EnemyMeleeAttack2State(this, "meleeAttack2");
        dodgeAttackState = new EnemyDodgeAttackState(this, "dodgeAttack");
        wideAttackState = new EnemyWideAttackState(this, "wideAttack");
        dashAttackState = new EnemyDashAttackState(this, "dashAttack");
        blockParryState = new EnemyBlockParryState(this, "blockParry");
        deadState = new EnemyDeadState(this, "dead");

        targetInDetectionRangeState = new EnemyTargetInDetectionRangeState(this, "move");

        abilityStates = new List<EnemyAbilityState>();
        IEnumerable<PropertyInfo> abilityStateProperties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.PropertyType.IsSubclassOf(typeof(EnemyAbilityState)));

        foreach (PropertyInfo property in abilityStateProperties)
        {
            EnemyAbilityState abilityState = property.GetValue(this) as EnemyAbilityState;
            abilityStates.Add(abilityState);
        }

        enemyStateMachine.Initialize(idleState);

        /*jumpState = new PlayerJumpState(this, "inAir");
        inAirState = new PlayerInAirState(this, "inAir");
        dodgeState = new PlayerDodgeState(this, "dodge");
        deadState = new PlayerDeadState(this, "dead");
        
        landingState = new PlayerLandingState(this, "landing");
        */
    }
}
