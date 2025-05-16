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
    

    protected override void Awake()
    {
        base.Awake();

        player = entity as Player;
    }

    public override void WhenGotHit()
    {

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
