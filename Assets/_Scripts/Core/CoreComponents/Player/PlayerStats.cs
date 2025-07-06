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

        experience.OnCurrentValueMax += () => { power.IncreaseCurrentValue(1); };
        power.OnCurrentValueChange += () =>
        {
            health.IncreaseMaxValue(health.graph.incrementPerLevel.Evaluate(power.currentValue));
            health.IncreaseCurrentValue(health.graph.incrementPerLevel.Evaluate(power.currentValue));
            posture.IncreaseMaxValue(posture.graph.incrementPerLevel.Evaluate(power.currentValue));
            posture.IncreaseCurrentValue(posture.graph.incrementPerLevel.Evaluate(power.currentValue));
        };
    }

    public void LoadData(GameData data)
    {
        // TODO: Implement LoadData
    }

    public void SaveData(GameData data)
    {
        data.currentLevel = (int)power.currentValue;
        data.currentHealth = health.currentValue;
        data.currentPosture = posture.currentValue;
    }
}
