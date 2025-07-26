using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StrongAttackEffect", menuName = "Scriptable Object/Item/Item Effect/Strong Attack Effect")]
public class StrongAttackEffect : ItemEffect
{
    const int maxLevel = 4;
    public override void ApplyEffect(Player target)
    {

        float currentPostureLevel = target.stats.power.currentValue;

        if (currentPostureLevel < (float)maxLevel)
        {
            target.stats.power.SetCurrentValue(currentPostureLevel + 1);
            Debug.Log($"current postureLevel : {target.stats.power.currentValue}");
        }
    }
}
