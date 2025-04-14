using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : Stats
{
    [field: SerializeField] public StatComponent detectionRatio { get; protected set; }

    private Enemy enemy;

    protected override void Awake()
    {
        base.Awake();

        enemy = entity as Enemy;
    }

    protected virtual void Start()
    {
        // health.OnCurrentValueMin += () => { enemy.enemyStateMachine.ChangeState(enemy.deadState); };
        // detectionRatio.OnCurrentValueMax += () => { enemy.enemyStateMachine.ChangeState(enemy.targetInDetectionRangeState); };
    }
}
