using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockParryComponent : CombatAbilityComponent
{
    [field: SerializeField] public BlockParryArea[] blockParryAreas { get; private set; }

    public override void ApplyCombatAbility(Entity target, OverlapCollider[] overlapColliders)
    {
        pertainedCombatAbility.sourceEntity.entityCombat.ReleaseBlockParryPrefabs();

        /*if (blockParryAreas.Count() > overlapColliders.Count())
        {
            Debug.LogWarning($"ShieldParryComponent of {pertainedCombatAbility.sourceEntity.name}'s {pertainedCombatAbility.name} combat ability does not match length between given overlapColliders({overlapColliders.Count()}) and shieldParryAreas({shieldParryAreas.Count()}). Oversized shieldParryAreas will be ignored.");
        }*/

        for (int index = 0; index < overlapColliders.Count(); index++)
        {
            BlockParryArea shieldParryArea = blockParryAreas[Mathf.Min(index, blockParryAreas.Count() - 1)];
            GameObject shieldParryPrefab = Manager.Instance.objectPoolingManager.GetGameObject("ShieldParryPrefab");
            shieldParryPrefab.transform.SetParent(pertainedCombatAbility.sourceEntity.entityCombat.transform);
            BlockParry shieldParry = shieldParryPrefab.GetComponent<BlockParry>();

            if (shieldParryArea.blockParryType.Equals(BlockParryType.Parry))
            {
                shieldParry.SetParryData(pertainedCombatAbility, shieldParryArea.parryTime[shieldParryArea.currentIndex], shieldParryArea.parryDurationTime[shieldParryArea.currentIndex], shieldParryArea.changeToBlock, overlapColliders[index]);
            }
            else
            {
                shieldParry.SetBlockData(pertainedCombatAbility, overlapColliders[index]);
            }

            if (Time.time != shieldParryArea.lastCalledTime)
            {
                if (Time.time - shieldParryArea.lastCalledTime < shieldParryArea.parryTimeDecrementReset)
                {
                    shieldParryArea.currentIndex += 1;
                    shieldParryArea.currentIndex = Mathf.Clamp(0, shieldParryArea.parryTime.Count() - 1, shieldParryArea.currentIndex);
                }
                else
                {
                    shieldParryArea.currentIndex = 0;
                }
                shieldParryArea.lastCalledTime = Time.time;
            }
        }
    }
}
