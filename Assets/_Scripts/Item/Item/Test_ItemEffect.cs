using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_ItemEffect : MonoBehaviour
{
    public PermanentHealEffect permenthealEffect;
    public HealEffect healEffect;
    public SpeedEffect speedEffect;
    public StrongAttackEffect attackEffect;

    public Player player;
    
    public void SpeedEffectItem()
    {
        speedEffect.ApplyEffect(player);
    }

    public void PermanentHealEffectItem()
    {
        permenthealEffect.ApplyEffect(player);
    }

    public void HealEffectItem()
    {
        healEffect.ApplyEffect(player);
    }

    public void StrongAttackEffectItem()
    {
        attackEffect.ApplyEffect(player);
    }

    public void DecreasePlayerHP()
    {
        player.stats.health.DecreaseCurrentValue(10f);
    }


}
