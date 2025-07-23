using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PermanentHealEffect", menuName = "Scriptable Object/Item/Item Effect/Permanent Heal Effect")]
public class PermanentHealEffect : ItemEffect
{
    // [SerializeField] private float percentage = 1.0f;
    public override void ApplyEffect(Player target)
    {
        float prevHealthMax = target.stats.health.maxValue;
        float currentHealLevel = target.stats.healthLevel.currentValue ;

        target.stats.healthLevel.SetCurrentValue(currentHealLevel + 1f);

        Debug.Log($"[Permanent Heal Effect] current healthLevel : {target.stats.healthLevel.currentValue}, max health : {prevHealthMax} -> {target.stats.health.maxValue} ");
    }
}
