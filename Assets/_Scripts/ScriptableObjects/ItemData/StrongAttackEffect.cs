using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StrongAttackEffect", menuName = "Scriptable Object/Item/Item Effect/Strong Attack Effect")]
public class StrongAttackEffect : ItemEffect
{
    //[SerializeField] private float percentage = 0.2f;
    public override void ApplyEffect(Player target)
    {

        // TODO : 강공격 시 방어력 감소량으로 변경해야 한다 
        
        // 강공격 및 적 방어력 속성에 대해 알아야 한다 

/*        float healAmount = target.stats.health.currentValue * 0.3f;
        target.stats.health.IncreaseCurrentValue(healAmount);*/
    }
}
