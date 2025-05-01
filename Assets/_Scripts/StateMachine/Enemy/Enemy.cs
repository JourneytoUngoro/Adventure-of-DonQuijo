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
    public EnemyMeleeAttack3State meleeAttack3State { get; private set; }
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
        meleeAttack0State = new EnemyMeleeAttack0State(this, "meleeAttack");
        meleeAttack1State = new EnemyMeleeAttack1State(this, "meleeAttack");
        meleeAttack2State = new EnemyMeleeAttack2State(this, "move");
        meleeAttack3State = new EnemyMeleeAttack3State(this, "meleeAttack");
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
