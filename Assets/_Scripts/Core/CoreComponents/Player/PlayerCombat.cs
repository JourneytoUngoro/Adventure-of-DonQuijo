using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCombat : Combat
{
    [field: SerializeField] public List<CombatAbilityWithColliders> dashAttack { get; private set; }
    [field: SerializeField] public List<CombatAbilityWithColliders> strongDashAttack { get; private set; }
    [field: SerializeField] public List<CombatAbilityWithColliders> meleeAttack { get; private set; }
    [field: SerializeField] public CombatAbilityWithColliders strongAttack { get; private set; }
    [field: SerializeField] public CombatAbilityWithColliders blockParry { get; private set; }
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

    public override void GetKnockback(KnockbackComponent knockbackComponent, OverlapCollider[] overlapColliders)
    {
        if (!player.playerStateMachine.currentState.Equals(player.stunnedState))
        {
            base.GetKnockback(knockbackComponent, overlapColliders);
        }
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
                    isParrying = parry.overlapCollider.limitAngle ? CheckWithinAngle(entity.orthogonalRigidbody.transform.right, sourceEntity.entityDetection.currentProjectedPosition - entity.entityDetection.currentProjectedPosition, parry.overlapCollider.counterClockwiseAngle, parry.overlapCollider.clockwiseAngle) : true;
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
                    isBlocking = block.overlapCollider.limitAngle ? CheckWithinAngle(entity.orthogonalRigidbody.transform.right, sourceEntity.entityDetection.currentProjectedPosition - entity.entityDetection.currentProjectedPosition, block.overlapCollider.counterClockwiseAngle, block.overlapCollider.clockwiseAngle) : true;
                }
                if (isBlocking) break;
            }
        }

        return isBlocking;
    }

    public void DoAlert(CombatAbilityWithColliders combatAbilityWithColliders)
    {
        List<Collider2D> damageTargets = new List<Collider2D>();
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(whatIsDamageable);
        contactFilter.useLayerMask = true;

        foreach (OverlapCollider overlapCollider in combatAbilityWithColliders.overlapColliders)
        {
            Array.Clear(detectedDamageTargets, 0, maxDetectionCount);
            overlapCollider.overlapCollider.OverlapCollider(contactFilter, detectedDamageTargets);
            damageTargets = damageTargets
                .Union(detectedDamageTargets.Where(target => {
                    if (target == null) return false;
                    Entity targetEntity = target.GetComponentInParent<Entity>();
                    float targetEntityFeetHeight = targetEntity.entityDetection.currentEntityHeight;
                    float targetEntityHeadHeight = targetEntityFeetHeight + targetEntity.currentEntityStature;
                    float colliderBottomHeight = overlapCollider.overlapCollider.gameObject.transform.position.z + entity.entityDetection.currentEntityHeight;
                    float colliderTopHeight = colliderBottomHeight + overlapCollider.height;
                    return !(targetEntityHeadHeight <= colliderBottomHeight || colliderTopHeight <= targetEntityFeetHeight);
                })).ToList();
        }

        foreach (Collider2D damageTarget in damageTargets)
        {
            Entity alertTarget = damageTarget.gameObject.GetComponentInParent<Entity>();

            // alertTarget?.combat.Alerted(player, combatAbilityWithColliders.combatAbilityData);
            alertTarget?.SetStatusValues(CurrentStatus.Alerted);
        }
    }
}
