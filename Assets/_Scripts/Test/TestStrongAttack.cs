using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStrongAttack : MonoBehaviour
{
    public CombatAbility strongAttack;

    public Player player;
    public Enemy enemy;

    public void ExecuteStrongAttack()
    {
        DamageComponent damageComponent = (DamageComponent) strongAttack.combatAbilityComponents[0];

        // 25 -> 30 -> 36 -> 43.2
        float postureDamage = damageComponent.postureDamage.accumulationPerLevel.Evaluate(player.stats.power.currentValue);
        Debug.Log($"postureDamage lv {player.stats.power.currentValue} : {postureDamage}");
    }
}