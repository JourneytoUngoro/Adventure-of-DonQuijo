using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Stats, IDataPersistance
{
    [field: SerializeField] public StatComponent experience { get; protected set; }
    [field: SerializeField] public StatComponent statPoints { get; protected set; }

    private Player player;

    protected override void Awake()
    {
        base.Awake();

        player = entity as Player;
    }

    protected override void Start()
    {
        base.Start();

        experience.OnCurrentValueMax += () => { level.IncreaseCurrentValue(1); };
        level.OnCurrentValueChange += () =>
        { 
            statPoints.IncreaseCurrentValue(player.playerData.statPointsPerLevel);
            experience.SetMaxValue(experience.graph.accumulationPerLevel.Evaluate(level.currentValue));
        };

        health.OnCurrentValueMin += () => { player.playerStateMachine.ChangeState(player.deadState); };
        // posture.OnCurrentValueMin += () => { player.playerStateMachine.ChangeState(player.stunnedState); };

        durability.OnCurrentValueChange += () =>
        {
            health.SetMaxValue(durability.graph.incrementPerLevel.Evaluate(durability.currentValue));
            health.IncreaseCurrentValue(health.graph.incrementPerLevel.Evaluate(durability.currentValue));
        };

        power.OnCurrentValueChange += () =>
        {
            posture.IncreaseMaxValue(posture.graph.incrementPerLevel.Evaluate(power.currentValue));
            posture.IncreaseCurrentValue(posture.graph.incrementPerLevel.Evaluate(power.currentValue));
        };
    }

    public void LoadData(GameData data)
    {
        health.SetMaxValue(data.maxHealth); // In case new game
        health.SetCurrentValue(data.currentHealth);
        durability.SetCurrentValue(data.currentHealthLevel);
        posture.SetCurrentValue(data.currentPosture);
        power.SetCurrentValue(data.postureLevel);
        agility.SetCurrentValue(data.moveSpeed);
    }

    public void SaveData(GameData data)
    {
        data.maxHealth = health.maxValue;
        data.currentHealth = health.currentValue;
        data.currentHealthLevel = durability.currentValue;
        data.currentPosture = posture.currentValue;
        data.postureLevel = power.currentValue;
        data.moveSpeed = agility.currentValue;
    }
}
