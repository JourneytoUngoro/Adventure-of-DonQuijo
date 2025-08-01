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

    public EnemyIdleState idleState { get; protected set; }
    public EnemyKnockbackState knockbackState { get; private set; }
    public EnemyStunnedState stunnedState { get; private set; }
    public EnemyTargetInDetectionRangeState targetInDetectionRangeState { get; protected set; }
    // public EnemyRangedAttckState rangedAttckState { get; private set; }
    public EnemyBlockParryState blockParryState { get; private set; }
    public EnemyDeadState deadState { get; private set; }
    public List<EnemyAbilityState> abilityStates { get; protected set; }
    #endregion

    #region Enemy Components
    public EnemyMovement movement { get; private set; }
    public EnemyDetection detection { get; private set; }
    public EnemyStats stats { get; private set; }
    public EnemyCombat combat { get; protected set; }
    public EnemyData enemyData { get; private set; }
    public NavMeshAgent navMeshAgent { get; private set; }
    #endregion

    private Transform initialTransform;

    protected override void Awake()
    {
        base.Awake();

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        initialTransform = transform;
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

        stunnedState = new EnemyStunnedState(this, "stunned");
        knockbackState = new EnemyKnockbackState(this, "knockback");
        deadState = new EnemyDeadState(this, "dead");

        abilityStates = new List<EnemyAbilityState>();
        /*jumpState = new PlayerJumpState(this, "inAir");
        inAirState = new PlayerInAirState(this, "inAir");
        dodgeState = new PlayerDodgeState(this, "dodge");
        deadState = new PlayerDeadState(this, "dead");
        
        landingState = new PlayerLandingState(this, "landing");
        */
    }

    public virtual void Revive()
    {
        transform.position = initialTransform.position;
        stats.health.SetCurrentValue(stats.health.maxValue);
        stats.posture.SetCurrentValue(stats.posture.maxValue);
    }
}
