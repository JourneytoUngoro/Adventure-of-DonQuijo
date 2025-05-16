using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockParryComponent : CombatAbilityComponent
{
    [field: SerializeField] public BlockParryArea[] blockParryAreas { get; private set; }

    public override void ApplyCombatAbility(Collider2D target, OverlapCollider[] overlapColliders)
    {
        pertainedCombatAbility.sourceEntity.entityCombat.DisableBlockParryPrefabs();

        for (int index = 0; index < overlapColliders.Count(); index++)
        {
            BlockParryArea blockParryArea = blockParryAreas[Mathf.Min(index, blockParryAreas.Count() - 1)];
            GameObject blockParryGameObject = overlapColliders[index].overlapCollider.gameObject;
            BlockParry blockParry = blockParryGameObject.GetComponent<BlockParry>();

            if (blockParryArea.blockParryType.Equals(BlockParryType.Parry))
            {
                blockParry.SetParryData(pertainedCombatAbility, blockParryArea.parryTime[blockParryArea.currentIndex], blockParryArea.parryDurationTime[blockParryArea.currentIndex], blockParryArea.changeToBlock, overlapColliders[index]);
            }
            else
            {
                blockParry.SetBlockData(pertainedCombatAbility, overlapColliders[index]);
            }

            if (Time.time != blockParryArea.lastCalledTime)
            {
                if (Time.time - blockParryArea.lastCalledTime < blockParryArea.parryTimeDecrementReset)
                {
                    blockParryArea.currentIndex += 1;
                    blockParryArea.currentIndex = Mathf.Clamp(0, blockParryArea.parryTime.Count() - 1, blockParryArea.currentIndex);
                }
                else
                {
                    blockParryArea.currentIndex = 0;
                }
                blockParryArea.lastCalledTime = Time.time;
            }
        }
    }
}
