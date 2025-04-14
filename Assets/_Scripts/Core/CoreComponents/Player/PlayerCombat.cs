using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : Combat
{
    [field: SerializeField] public CombatAbilityWithColliders blockParryArea { get; private set; }
    [field: SerializeField] public CombatAbilityWithColliders aerialBlockParryArea { get; private set; }

    private Player player;

    protected override void Awake()
    {
        base.Awake();

        player = entity as Player;
    }

    protected override void ChangeToKnockbackState(float knockbackTime)
    {
        throw new System.NotImplementedException();
    }
}
