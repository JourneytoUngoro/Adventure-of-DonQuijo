using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeedEffect", menuName = "Scriptable Object/Item/Item Effect/Speed Effect")]
public class SpeedEffect : ItemEffect
{ 
    [SerializeField] private float percentage = 0.1f;
    public override void ApplyEffect(Player target)
    {
        // Increases player's max speed by 10%
        float currentSpeedValue = target.stats.speed.currentValue;
        float speedAmount = target.stats.speed.maxValue * percentage;

        target.stats.speed.IncreaseMaxValue(speedAmount);

        Debug.Log($"apply speed effect :{currentSpeedValue} + {speedAmount}  = {target.stats.speed.currentValue} ");
    }
}