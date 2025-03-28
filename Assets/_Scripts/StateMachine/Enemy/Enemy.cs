using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Enemy : Entity
{
    #region State Variables
    public EnemyStateMachine enemyStateMachine;

    #endregion

    #region Enemy Components
    public EnemyMovement movement { get; private set; }
    public EnemyDetection detection { get; private set; }
    public EnemyStats stats { get; private set; }
    public EnemyData enemyData { get; private set; }
    #endregion

    protected override void Start()
    {
        base.Start();

        movement = entityMovement as EnemyMovement;
        detection = entityDetection as EnemyDetection;
        stats = entityStats as EnemyStats;
        enemyData = entityData as EnemyData;

        enemyStateMachine = new EnemyStateMachine();
        entityStateMachine = enemyStateMachine;

        /*idleState = new PlayerIdleState(this, "idle");
        moveState = new PlayerMoveState(this, "move");
        jumpState = new PlayerJumpState(this, "inAir");
        inAirState = new PlayerInAirState(this, "inAir");
        dodgeState = new PlayerDodgeState(this, "dodge");
        deadState = new PlayerDeadState(this, "dead");
        stunnedState = new PlayerStunnedState(this, "stunned");
        landingState = new PlayerLandingState(this, "landing");
        knockbackState = new PlayerKnockbackState(this, "knockback");

        abilityStates = new List<PlayerAbilityState>();
        IEnumerable<PropertyInfo> abilityStateProperties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.PropertyType.IsSubclassOf(typeof(PlayerAbilityState)));

        foreach (PropertyInfo property in abilityStateProperties)
        {
            PlayerAbilityState abilityState = property.GetValue(this) as PlayerAbilityState;
            abilityStates.Add(abilityState);
        }

        playerStateMachine.Initialize(idleState);*/
    }
}
