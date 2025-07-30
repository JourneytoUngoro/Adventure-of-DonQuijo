using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceCombat : EnemyCombat
{
    [field: SerializeField] public List<CombatAbilityWithColliders> meleeAttack { get; private set; }
}
