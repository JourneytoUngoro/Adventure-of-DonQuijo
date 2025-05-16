using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Stats, IDataPersistance
{
    [field: SerializeField] public StatComponent experience { get; protected set; }

    private Player player;

    protected override void Awake()
    {
        base.Awake();

        player = entity as Player;
    }

    protected override void Start()
    {
        base.Start();

        health.OnCurrentValueMin += () => { player.playerStateMachine.ChangeState(player.deadState); };
        // posture.OnCurrentValueMin += () => { player.playerStateMachine.ChangeState(player.stunnedState); };

        experience.OnCurrentValueMax += () => { level.IncreaseCurrentValue(1); };
        level.OnCurrentValueChange += () =>
        {
            health.IncreaseMaxValue(health.graph.incrementPerLevel.Evaluate(level.currentValue));
            health.IncreaseCurrentValue(health.graph.incrementPerLevel.Evaluate(level.currentValue));
            posture.IncreaseMaxValue(posture.graph.incrementPerLevel.Evaluate(level.currentValue));
            posture.IncreaseCurrentValue(posture.graph.incrementPerLevel.Evaluate(level.currentValue));
        };
    }

    public void LoadData(GameData data)
    {
        // TODO: Implement LoadData
    }

    public void SaveData(GameData data)
    {
        data.currentLevel = (int)level.currentValue;
        data.currentHealth = health.currentValue;
        data.currentPosture = posture.currentValue;
    }
}
