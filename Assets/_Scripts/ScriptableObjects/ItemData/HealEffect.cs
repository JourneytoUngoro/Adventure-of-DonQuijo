using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealEffect", menuName = "Scriptable Object/Item/Item Effect/Heal Effect")]
public class HealEffect : ItemEffect
{
    [SerializeField] private float percentage = 1.0f;
    public override void ApplyEffect(Player target)
    {
        float currentHealValue = target.stats.health.currentValue;
        float additionalHealAmount = target.stats.health.currentValue * percentage;

        target.stats.health.IncreaseCurrentValue(additionalHealAmount);

        Debug.Log($"apply heal effect : {currentHealValue} + {additionalHealAmount}  = {target.stats.health.currentValue} ");
    }
}
