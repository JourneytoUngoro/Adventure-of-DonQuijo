using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class CombatAbilityComponent: ICombatAbility
{
    [SerializeField, HideInInspector] private string name = "";
    [field: SerializeField] public CombatAbility pertainedCombatAbility { get; set; }

    public CombatAbilityComponent()
    {
        name = GetType().Name;
    }

    public abstract void ApplyCombatAbility(Entity target, OverlapCollider[] overlapColliders);
}