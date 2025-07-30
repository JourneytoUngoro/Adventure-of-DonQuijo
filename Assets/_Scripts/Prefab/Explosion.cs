using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Explosion : PooledObject
{
    [field: SerializeField] public List<CombatAbilityWithColliders> combatAbilityWithCollidersList { get; private set; }
    [SerializeField] private Transform knockbackSourceTransform;
    [SerializeField] private LayerMask whatIsDamageable;

    public Entity sourceEntity { get; protected set; }
    public Projectile sourceProjectile { get; protected set; }

    private List<Collider2D> damagedTargets = new List<Collider2D>();
    private ContactFilter2D explosionContactFilter;
    private const int maxDetectionCount = 10;

    private void Awake()
    {
        foreach (CombatAbilityWithColliders combatAbilityWithColliders in combatAbilityWithCollidersList)
        {
            CombatAbility combatAbility = combatAbilityWithColliders.combatAbilityData;

            foreach (CombatAbilityComponent combatAbilityComponent in combatAbility.combatAbilityComponents)
            {
                combatAbilityComponent.pertainedCombatAbility = combatAbility;

                if (combatAbilityComponent.GetType().Equals(typeof(KnockbackComponent)))
                {
                    KnockbackComponent knockbackComponent = combatAbilityComponent as KnockbackComponent;
                    knockbackComponent.knockbackSourceTransform = knockbackSourceTransform ? knockbackSourceTransform : transform;
                }
            }
        }
    }

    private void Start()
    {
        explosionContactFilter = new ContactFilter2D();
        explosionContactFilter.useLayerMask = true;
        explosionContactFilter.useTriggers = true;
        explosionContactFilter.SetLayerMask(whatIsDamageable);
    }

    public override void ReleaseObject()
    {
        Destroy(gameObject);
        /*base.ReleaseObject();

        sourceEntity = null;
        sourceProjectile = null;
        damagedTargets.Clear();*/
    }

    public void SetExplosion(Entity sourceEntity)
    {
        this.sourceEntity = sourceEntity;

        foreach (CombatAbilityWithColliders combatAbilityWithColliders in combatAbilityWithCollidersList)
        {
            CombatAbility combatAbility = combatAbilityWithColliders.combatAbilityData;
            combatAbility.sourceEntity = sourceEntity;
        }
    }

    public void SetExplosion(Projectile sourceProjectile)
    {
        this.sourceProjectile = sourceProjectile;
        this.sourceEntity = sourceProjectile.sourceEntity;
        transform.position = sourceProjectile.transform.position;

        foreach (CombatAbilityWithColliders combatAbilityWithColliders in combatAbilityWithCollidersList)
        {
            CombatAbility combatAbility = combatAbilityWithColliders.combatAbilityData;
            combatAbility.sourceEntity = sourceProjectile.sourceEntity;
        }
    }

    public void Explode(int index)
    {
        Collider2D[] tempTargets = new Collider2D[maxDetectionCount];
        HashSet<Collider2D> damageTargets = new HashSet<Collider2D>();

        CombatAbilityWithColliders combatAbilityWithColliders = combatAbilityWithCollidersList[index];

        foreach (OverlapCollider overlapCollider in combatAbilityWithColliders.overlapColliders)
        {
            int count = overlapCollider.collider.OverlapCollider(explosionContactFilter, tempTargets);
            
            for (int i = 0; i < count; i++)
            {
                damageTargets.Add(tempTargets[i]);
            }
        }

        foreach (Collider2D damageTarget in damageTargets)
        {
            if (damagedTargets.Contains(damageTarget)) continue;
            if (damageTarget.CompareTag("Invinsible")) continue;
            if (combatAbilityWithColliders.combatAbilityData.canBeDodged && damageTarget.CompareTag("Dodge")) continue;

            foreach (CombatAbilityComponent combatAbilityComponent in combatAbilityWithColliders.combatAbilityData.combatAbilityComponents)
            {
                switch (combatAbilityComponent)
                {
                    case DamageComponent damageComponent:
                        damageComponent.ApplyCombatAbility(damageTarget, combatAbilityWithColliders.overlapColliders);
                        break;
                    case KnockbackComponent knockbackComponent:
                        knockbackComponent.ApplyCombatAbility(damageTarget, combatAbilityWithColliders.overlapColliders);
                        break;
                    default:
                        break;
                }
            }

            damagedTargets.Add(damageTarget);
        }
    }
}
