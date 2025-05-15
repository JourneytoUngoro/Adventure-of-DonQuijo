using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatAbilityWithColliders
{
    [field: SerializeField] public bool visualize { get; private set; } = true;
    [field: SerializeField] public OverlapCollider[] overlapColliders { get; private set; }
    [field: SerializeField] public CombatAbility combatAbilityData { get; private set; }

    public CombatAbilityWithColliders(bool visualize, OverlapCollider[] overlapColliders, CombatAbility combatAbilityData)
    {
        this.visualize = visualize;
        this.overlapColliders = overlapColliders;
        this.combatAbilityData = combatAbilityData;
    }
}
