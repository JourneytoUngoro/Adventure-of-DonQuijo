using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatCombat : EnemyCombat
{
    [field: SerializeField] public List<CombatAbilityWithColliders> meleeAttack { get; private set; }
}
