using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorCombat : EnemyCombat
{
    [field: SerializeField] public List<CombatAbilityWithColliders> rangedAttack { get; private set; }
}
