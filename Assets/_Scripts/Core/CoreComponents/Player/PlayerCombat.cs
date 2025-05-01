using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCombat : Combat
{
    [field: SerializeField] public CombatAbilityWithColliders blockParry { get; private set; }
    [field: SerializeField] public List<CombatAbilityWithColliders> meleeAttack { get; private set; }
    // [field: SerializeField] public CombatAbilityWithColliders aerialBlockParryArea { get; private set; }

    private Player player;
    private ContactFilter2D blockParryContactFilter;

    protected override void Awake()
    {
        base.Awake();

        player = entity as Player;

        blockParryContactFilter = new ContactFilter2D();
        blockParryContactFilter.useLayerMask = true;
        blockParryContactFilter.useTriggers = true;
    }

    protected override void ChangeToKnockbackState(float knockbackTime)
    {
        player.knockbackState.knockbackTimer.ChangeDuration(knockbackTime);
        player.playerStateMachine.ChangeState(player.knockbackState);
    }

    public override bool IsParrying(Entity sourceEntity, OverlapCollider[] overlapColliders)
    {
        bool isParrying = false;

        if (sourceEntity != null)
        {
            List<Collider2D> parryColliders = new List<Collider2D>();
            blockParryContactFilter.SetLayerMask(LayerMask.GetMask("ParryLayer"));

            foreach (OverlapCollider overlapCollider in overlapColliders)
            {
                overlapCollider.overlapCollider.OverlapCollider(blockParryContactFilter, parryColliders);
                BlockParry parry = parryColliders.Select(collider => collider.GetComponent<BlockParry>()).Where(blockParry => blockParry.pertainedCombatAbility.sourceEntity.Equals(entity)).FirstOrDefault();

                if (parry != null)
                {
                    isParrying = parry.overlapCollider.limitAngle ? CheckWithinAngle(entity.transform.right, sourceEntity.transform.position - entity.transform.position, parry.overlapCollider.counterClockwiseAngle, parry.overlapCollider.clockwiseAngle) : true;
                }

                if (isParrying) break;
            }
        }

        return isParrying;
    }

    public override bool IsBlocking(Entity sourceEntity, OverlapCollider[] overlapColliders)
    {
        bool isBlocking = false;

        if (sourceEntity != null)
        {
            List<Collider2D> blockColliders = new List<Collider2D>();
            blockParryContactFilter.SetLayerMask(LayerMask.GetMask("BlockLayer"));

            foreach (OverlapCollider overlapCollider in overlapColliders)
            {
                overlapCollider.overlapCollider.OverlapCollider(blockParryContactFilter, blockColliders);
                BlockParry block = blockColliders.Select(collider => collider.GetComponent<BlockParry>()).Where(blockParry => blockParry.pertainedCombatAbility.sourceEntity.Equals(entity)).FirstOrDefault();

                if (block != null)
                {
                    isBlocking = block.overlapCollider.limitAngle ? CheckWithinAngle(entity.transform.right, sourceEntity.transform.position - entity.transform.position, block.overlapCollider.counterClockwiseAngle, block.overlapCollider.clockwiseAngle) : true;
                }

                if (isBlocking) break;
            }
        }

        return isBlocking;
    }
}
