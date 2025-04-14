using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeedEffect", menuName = "Scriptable Object/Item/Item Effect/Speed Effect")]
public class SpeedEffect : ItemEffect
{ 
    [SerializeField] private float percentage = 0.1f;
    public override void ApplyEffect(Player target)
    {
        // TODO : speed로 변경해야 한다 

        // 최대 속도 10% 증가
        float debugging = target.stats.speed.currentValue;

        float speedAmount = target.stats.speed.maxValue * percentage;

        target.stats.speed.IncreaseMaxValue(speedAmount);

        Debug.Log($"Speed Effect : {debugging} + {speedAmount}  = {target.stats.speed.currentValue} ");
    }
}