using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : EntityState
{
    public Enemy enemy { get; private set; }
    protected EnemyData enemyData;
    protected EnemyStateMachine stateMachine;

    #region Shared Detection
    protected bool isTargetInDetectionRange;
    #endregion

    public EnemyState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        this.enemy = enemy;
        enemyData = enemy.enemyData;
        stateMachine = enemy.enemyStateMachine;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isTargetInDetectionRange = enemy.detection.isTargetInDetectionRange();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (isGrounded)
        {
            enemy.orthogonalRigidbody.gravityScale = 0.0f;
        }
    }

    protected override void TickPublicTimers()
    {
        base.TickPublicTimers();

        enemy.knockbackState.knockbackTimer.Tick();

        /*foreach (EnemyAbilityState enemyAbilityState in enemy.abilityStates)
        {
            enemyAbilityState.abilityCoolDownTimer.Tick();
        }*/
        enemy.meleeAttack0State.abilityCoolDownTimer.Tick();
        enemy.meleeAttack1State.abilityCoolDownTimer.Tick();
        enemy.meleeAttack2State.abilityCoolDownTimer.Tick();
        enemy.meleeAttack3State.abilityCoolDownTimer.Tick();
    }
}
