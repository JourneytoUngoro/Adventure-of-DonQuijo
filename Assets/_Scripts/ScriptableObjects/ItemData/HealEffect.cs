using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealEffect", menuName = "Scriptable Object/Item/Item Effect/Heal Effect")]
public class HealEffect : ItemEffect
{
    [SerializeField] private float percentage = 0.3f;
    public override void ApplyEffect(Player target)
    {
        float currentHealth = target.stats.health.currentValue;
        float additionalHealAmount = target.stats.health.maxValue * percentage;

        target.stats.health.IncreaseCurrentValue(additionalHealAmount);

        Debug.Log($"apply heal effect : {currentHealth} + {additionalHealAmount}  = {target.stats.health.currentValue} ");
    }
}
