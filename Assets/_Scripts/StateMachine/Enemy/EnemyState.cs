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

    protected override void TickPublicTimers()
    {
        base.TickPublicTimers();


    }
}
