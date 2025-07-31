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
            health.SetMaxValue(health.graph.accumulationPerLevel.Evaluate(durability.currentValue));
            health.IncreaseCurrentValue(health.graph.incrementPerLevel.Evaluate(durability.currentValue));
        };

        /*power.OnCurrentValueChange += () =>
        {
            posture.IncreaseMaxValue(posture.graph.incrementPerLevel.Evaluate(power.currentValue));
            posture.IncreaseCurrentValue(posture.graph.incrementPerLevel.Evaluate(power.currentValue));
        };*/
    }

    public void LoadData(GameData data)
    {
        health.SetMaxValue(data.maxHealth, false);
        health.SetCurrentValue(data.currentHealth, true, false, false);
        durability.SetCurrentValue(data.currentDurability, true, false, false);
        posture.SetMaxValue(data.maxPosture, false);
        posture.SetCurrentValue(data.currentPosture, true, false, false);
        power.SetCurrentValue(data.currentPower, true, false, false);
        agility.SetCurrentValue(data.currentAgility, true, false, false);
    }

    public void SaveData(GameData data)
    {
        data.currentHealth = health.currentValue;
        data.currentDurability = durability.currentValue;
        data.currentPosture = posture.currentValue;
        data.currentPower = power.currentValue;
        data.currentAgility = agility.currentValue;
    }
}
