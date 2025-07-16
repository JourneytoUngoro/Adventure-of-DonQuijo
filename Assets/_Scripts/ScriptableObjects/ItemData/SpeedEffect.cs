using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeedEffect", menuName = "Scriptable Object/Item/Item Effect/Speed Effect")]
public class SpeedEffect : ItemEffect
{ 
    [SerializeField] private float percentage = 0.15f;
    public override void ApplyEffect(Player target)
    {
        // Increases player's max speedLevel by 15%
        float currentSpeedValue = target.stats.speedLevel.currentValue;
        float speedAmount = target.stats.speedLevel.currentValue * percentage;

        // Used up to 3 times; exceeds won't break max value, but max must be adjusted if usage increases.
        target.stats.speedLevel.IncreaseCurrentValue(speedAmount);

        Debug.Log($"apply speed effect :{currentSpeedValue} + {speedAmount}  = {target.stats.speedLevel.currentValue} ");
    }
}