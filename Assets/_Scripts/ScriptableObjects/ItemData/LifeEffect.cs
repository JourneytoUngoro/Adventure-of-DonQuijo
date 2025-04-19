using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LifeEffect", menuName = "Scriptable Object/Item/Item Effect/Life Effect")]

public class LifeEffect : ItemEffect
{
    // [SerializeField] private int additionalLife= 1;
    public override void ApplyEffect(Player target)
    {

        // TODO : 추가 생명으로 바꿔야 한다 
        // 최대 채력의 50%로 부활한다 

        float debugging = target.stats.health.currentValue;

        float fullAmount = target.stats.health.maxValue * 0.5f;

        target.stats.health.SetCurrentValue(fullAmount);

        Debug.Log($"Heal Effect : + {debugging}  = {target.stats.health.currentValue} ");
    }
}
