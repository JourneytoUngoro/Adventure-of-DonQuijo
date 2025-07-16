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

        experience.OnCurrentValueMax += () => { postureLevel.IncreaseCurrentValue(1); };

        healthLevel.OnCurrentValueChange += () =>
        {
            Debug.Log($"evaluate amount : {health.graph.incrementPerLevel.Evaluate(healthLevel.currentValue)}");

            health.SetMaxValue(healthLevel.graph.incrementPerLevel.Evaluate(healthLevel.currentValue));
            health.IncreaseCurrentValue(health.graph.incrementPerLevel.Evaluate(healthLevel.currentValue));
        };

        postureLevel.OnCurrentValueChange += () =>
        {
            posture.IncreaseMaxValue(posture.graph.incrementPerLevel.Evaluate(postureLevel.currentValue));
            posture.IncreaseCurrentValue(posture.graph.incrementPerLevel.Evaluate(postureLevel.currentValue));
        };
    }

    public void LoadData(GameData data)
    {
        health.SetMaxValue(data.maxHealth); // In case new game
        health.SetCurrentValue(data.currentHealth);
        healthLevel.SetCurrentValue(data.currentHealthLevel);
        posture.SetCurrentValue(data.currentPosture);
        postureLevel.SetCurrentValue(data.postureLevel);
        speedLevel.SetCurrentValue(data.moveSpeed);
    }

    public void SaveData(GameData data)
    {
        data.maxHealth = health.maxValue;
        data.currentHealth = health.currentValue;
        data.currentHealthLevel = healthLevel.currentValue;
        data.currentPosture = posture.currentValue;
        data.postureLevel = postureLevel.currentValue;
        data.moveSpeed = speedLevel.currentValue;
    }
}
