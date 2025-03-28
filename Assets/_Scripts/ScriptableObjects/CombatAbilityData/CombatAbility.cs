using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Intensity { Ðñ, î¸, øÁ, êË, ß¯ }

[CreateAssetMenu(fileName = "newCombatAbilityData", menuName = "Data/Combat Ability Data")]
public class CombatAbility : ScriptableObject
{
    [field: SerializeField] public Sprite combatAbilityIcon { get; private set; }
    [field: SerializeField] public string combatAbilityName { get; private set; } = "Default Combat Ability Name";
    [field: SerializeField, Tooltip("When target gets knockback only when its stance level is lower than threat level.")] public Intensity threatLevel { get; private set; } = Intensity.øÁ;
    [field: SerializeField, TextArea] public string combatAbilityDescription { get; private set; } = "Default Combat Ability Description";
    [field: SerializeField] public bool canBeDodged { get; private set; } = true;
    [field: SerializeField] public bool canBeShielded { get; private set; } = true;
    [field: SerializeField] public bool canBeParried { get; private set; } = true;
    // TODO: Add stance & superstance logic
    [field: SerializeField, Tooltip("stanceWhenHit will be controlled by entity's stanceLevel by script.")] public bool stanceWhenParried { get; private set; } = false;
    // TODO: Add dazed logic
    [field: SerializeField] public bool getDazed { get; private set; }
    [field: SerializeReference] public List<CombatAbilityComponent> combatAbilityComponents { get; private set; }
    public Entity sourceEntity { get; set; }

    public void AddComponent(CombatAbilityComponent componentData)
    {
        componentData.pertainedCombatAbility = this;

        if (combatAbilityComponents.FirstOrDefault(type => type.GetType().Equals(componentData.GetType())) == null)
        {
            combatAbilityComponents.Add(componentData);
        }
    }
}
