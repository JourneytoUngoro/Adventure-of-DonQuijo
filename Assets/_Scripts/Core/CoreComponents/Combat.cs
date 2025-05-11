using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class Combat : CoreComponent
{
    [field: SerializeField] public LayerMask whatIsDamageable { get; protected set; }
    public List<Collider2D> damagedTargets { get; private set; }
    public List<Entity> surroundedBy { get; private set; } = new List<Entity>();
    public List<Entity> targetedBy { get; private set; } = new List<Entity>();
    public int stanceLevel { get; protected set; }

    protected const int maxDetectionCount = 10;

    private Collider2D[] detectedDamageTargets = new Collider2D[maxDetectionCount];

    protected override void Awake()
    {
        base.Awake();

        damagedTargets = new List<Collider2D>();

        IEnumerable<PropertyInfo> combatAbilityProperties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.PropertyType.Equals(typeof(List<CombatAbilityWithColliders>)));

        foreach (PropertyInfo property in combatAbilityProperties)
        {
            List<CombatAbilityWithColliders> combatAbilityList = property.GetValue(this) as List<CombatAbilityWithColliders>;

            foreach (CombatAbilityWithColliders combatAbilityWithTransforms in combatAbilityList)
            {
                combatAbilityWithTransforms.combatAbilityData.sourceEntity = entity;

                foreach (CombatAbilityComponent combatAbilityComponent in combatAbilityWithTransforms.combatAbilityData.combatAbilityComponents)
                {
                    combatAbilityComponent.pertainedCombatAbility = combatAbilityWithTransforms.combatAbilityData;

                    if (combatAbilityComponent.GetType().Equals(typeof(KnockbackComponent)))
                    {
                        KnockbackComponent knockbackComponent = combatAbilityComponent as KnockbackComponent;
                        knockbackComponent.knockbackSourceTransform = entity.transform;
                    }
                }
            }
        }

        combatAbilityProperties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.PropertyType.Equals(typeof(CombatAbilityWithColliders)));

        foreach (PropertyInfo property in combatAbilityProperties)
        {
            CombatAbilityWithColliders combatAbilityWithTransforms = property.GetValue(this) as CombatAbilityWithColliders;

            combatAbilityWithTransforms.combatAbilityData.sourceEntity = entity;

            foreach (CombatAbilityComponent combatAbilityComponent in combatAbilityWithTransforms.combatAbilityData.combatAbilityComponents)
            {
                combatAbilityComponent.pertainedCombatAbility = combatAbilityWithTransforms.combatAbilityData;
            }
        }
    }

    public virtual pair<bool, bool> DoAttack(CombatAbilityWithColliders combatAbilityWithColliders)
    {
        bool foundTarget = false;
        bool hitTarget = false;

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
                    float targetEntityHeadHeight = targetEntityFeetHeight + targetEntity.GetComponent<Entity>().entityData.entityHeight;
                    float colliderBottomHeight = overlapCollider.overlapCollider.gameObject.transform.position.z + entity.entityDetection.currentEntityHeight;
                    float colliderTopHeight = colliderBottomHeight + overlapCollider.height;
                    return colliderBottomHeight < targetEntityHeadHeight || colliderTopHeight > targetEntityFeetHeight;
                })).ToList();
        }

        damageTargets.Remove(entity.entityCollider);
        damageTargets.Remove(null);

        foreach (CombatAbilityComponent combatAbilityComponent in combatAbilityWithColliders.combatAbilityData.combatAbilityComponents)
        {
            switch (combatAbilityComponent)
            {
                case MovementComponent movementComponent:
                    movementComponent.ApplyCombatAbility(null, null);
                    break;
                case BlockParryComponent blockParryComponent:
                    blockParryComponent.ApplyCombatAbility(null, combatAbilityWithColliders.overlapColliders);
                    break;
                /*case ProjectileComponent projectileComponent:
                    Transform[] projectileFireTransforms = combatAbilityWithColliders.overlapColliders.Where(overlapCollider => !overlapCollider.overlapBox && !overlapCollider.overlapCircle).Select(overlapCollider => overlapCollider.centerTransform).ToArray();
                    projectileComponent.ApplyCombatAbility(damageTargets, projectileFireTransforms, null);
                    break;*/
                default: break;
            }
        }

        foreach (Collider2D damageTarget in damageTargets)
        {
            if (damagedTargets.Contains(damageTarget)) continue;
            if (damageTarget.CompareTag("Invinsible")) continue;
            if (combatAbilityWithColliders.combatAbilityData.canBeDodged && damageTarget.CompareTag("Dodge")) continue;

            foundTarget = true;
            Entity damageTargetEntity = damageTarget.GetComponent<Entity>();
            damageTargetEntity.SetStatusValues(CurrentStatus.gotHit);

            foreach (CombatAbilityComponent combatAbilityComponent in combatAbilityWithColliders.combatAbilityData.combatAbilityComponents)
            {
                switch (combatAbilityComponent)
                {
                    case DamageComponent damageComponent:
                        hitTarget = true;
                        damageComponent.ApplyCombatAbility(damageTarget, combatAbilityWithColliders.overlapColliders);
                        break;
                    case KnockbackComponent knockbackComponent:
                        hitTarget = true;
                        knockbackComponent.ApplyCombatAbility(damageTarget, combatAbilityWithColliders.overlapColliders);
                        break;
                    /*case StatusEffectComponent statusEffectComponent:
                        statusEffectComponent.ApplyCombatAbility(damageTarget, combatAbilityWithTransforms.overlapColliders);
                        break;*/
                    default: break;
                }
            }

            damagedTargets.Add(damageTarget);

            if (damageTargetEntity.GetType().IsSubclassOf(typeof(Enemy)) {
                Enemy damageTargetEnemy = damageTargetEntity as Enemy;
                damageTargetEnemy.detection.ChangeCurrentTarget(entity);
            }
        }

        return new pair<bool, bool>(foundTarget, hitTarget);
    }

    public void GetMovement(MovementComponent movementComponent)
    {
        Vector2 direction = Vector2.one;
        direction.x = entity.entityMovement.facingDirection;

        entity.entityMovement.SetVelocityChangeOverTime(movementComponent.planeDirection.normalized * direction, movementComponent.planeVelocity, movementComponent.moveTime, movementComponent.easeFunction, movementComponent.slowDown, false);
        entity.entityMovement.SetVelocityZ(movementComponent.orthogonalVelocity);
    }

    public virtual void GetDamage(DamageComponent damageComponent, OverlapCollider[] overlapColliders)
    {
        Entity sourceEntity = damageComponent.pertainedCombatAbility.sourceEntity;
        bool isParrying = damageComponent.pertainedCombatAbility.canBeParried ? IsParrying(sourceEntity, overlapColliders) : false;
        bool isBlocking = damageComponent.pertainedCombatAbility.canBeBlocked ? IsBlocking(sourceEntity, overlapColliders) : false;

        GetHealthDamage(damageComponent, isParrying, isBlocking);
        GetPostureDamage(damageComponent, isParrying, isBlocking);
    }

    public virtual void GetHealthDamage(DamageComponent damageComponent, bool isParrying, bool isBlocking)
    {
        Entity sourceEntity = damageComponent.pertainedCombatAbility.sourceEntity;
        float healthDamage = damageComponent.healthDamage.accumulationPerLevel.Evaluate(entity.entityStats.level.currentValue);

        if (damageComponent.pertainedCombatAbility.canBeParried)
        {
            if (isParrying)
            {
                Parried();
                entity.animator.SetTrigger("parried");
                entity.animator.SetInteger("typeIndex", UtilityFunctions.RandomInteger(3));
                entity.SetStatusValues(CurrentStatus.Parried);
                entity.entityStats.health.DecreaseCurrentValue(healthDamage * (1.0f - damageComponent.healthDamageParryRate));
                sourceEntity.SetStatusValues(CurrentStatus.wasParried);
                sourceEntity.entityStats.health.DecreaseCurrentValue(healthDamage * damageComponent.healthCounterDamageRate);
            }
            else
            {
                if (damageComponent.pertainedCombatAbility.canBeBlocked)
                {
                    if (isParrying || isBlocking)
                    {
                        entity.SetStatusValues(CurrentStatus.Blocked);
                        entity.animator.SetTrigger("blocked");
                        entity.entityStats.health.DecreaseCurrentValue(healthDamage * (1.0f - damageComponent.healthDamageShieldRate));
                    }
                    else
                    {
                        entity.SetStatusValues(CurrentStatus.HealthDamage);
                        // entity.animator.SetTrigger("gotHit");
                        entity.entityStats.health.DecreaseCurrentValue(healthDamage);
                    }
                }
                else
                {
                    entity.SetStatusValues(CurrentStatus.HealthDamage);
                    // entity.animator.SetTrigger("gotHit");
                    entity.entityStats.health.DecreaseCurrentValue(healthDamage);
                }
            }
        }
        else if (damageComponent.pertainedCombatAbility.canBeBlocked)
        {
            if (isParrying || isBlocking)
            {
                entity.SetStatusValues(CurrentStatus.Blocked);
                entity.animator.SetTrigger("blocked");
                entity.entityStats.health.DecreaseCurrentValue(healthDamage * (1.0f - damageComponent.healthDamageShieldRate));
            }
            else
            {
                entity.SetStatusValues(CurrentStatus.gotHit);
                // entity.animator.SetTrigger("gotHit");
                entity.entityStats.health.DecreaseCurrentValue(healthDamage);
            }
        }
        else
        {
            entity.SetStatusValues(CurrentStatus.gotHit);
            // entity.animator.SetTrigger("gotHit");
            entity.entityStats.health.DecreaseCurrentValue(healthDamage);
        }
    }

    public virtual void GetPostureDamage(DamageComponent damageComponent, bool isParrying, bool isShielding)
    {
        Entity sourceEntity = damageComponent.pertainedCombatAbility.sourceEntity;
        float postureDamage = damageComponent.postureDamage.accumulationPerLevel.Evaluate(entity.entityStats.level.currentValue);

        if (damageComponent.pertainedCombatAbility.canBeParried)
        {
            if (isParrying)
            {
                entity.entityStats.posture.IncreaseCurrentValue(postureDamage * (1.0f - damageComponent.postureDamageParryRate), false);
                sourceEntity.entityStats.posture.IncreaseCurrentValue(postureDamage * damageComponent.postureCounterDamageRate, !sourceEntity.GetType().Equals(typeof(Player)));
            }
            else
            {
                if (damageComponent.pertainedCombatAbility.canBeBlocked)
                {
                    if (isParrying || isShielding)
                    {
                        entity.entityStats.posture.IncreaseCurrentValue(postureDamage * (1.0f - damageComponent.postureDamageShieldRate));
                    }
                    else
                    {
                        entity.entityStats.posture.IncreaseCurrentValue(postureDamage);
                    }
                }
                else
                {
                    entity.entityStats.posture.IncreaseCurrentValue(postureDamage);
                }
            }
        }
        else if (damageComponent.pertainedCombatAbility.canBeBlocked)
        {
            if (isParrying || isShielding)
            {
                entity.entityStats.posture.IncreaseCurrentValue(postureDamage * (1.0f - damageComponent.postureDamageShieldRate));
            }
            else
            {
                entity.entityStats.posture.IncreaseCurrentValue(postureDamage);
            }
        }
        else
        {
            entity.entityStats.posture.IncreaseCurrentValue(postureDamage);
        }
    }

    // Notice: knockbackTime is only available when orthogonal velocity is set to zero. If knockbackTime is set to zero, the entity will recover from the knockback when it is grounded.
    public virtual void GetKnockback(KnockbackComponent knockbackComponent, OverlapCollider[] overlapColliders)
    {
        Entity sourceEntity = knockbackComponent.pertainedCombatAbility.sourceEntity;
        bool isParrying = knockbackComponent.pertainedCombatAbility.canBeParried ? IsParrying(sourceEntity, overlapColliders) : false;
        bool isBlocking = knockbackComponent.pertainedCombatAbility.canBeBlocked ? IsBlocking(sourceEntity, overlapColliders) : false;

        Vector2 directionMultiplier = Vector2.zero;

        switch (knockbackComponent.directionBase)
        {
            case DirectionBase.Position:
                directionMultiplier.x = entity.transform.position.x - knockbackComponent.knockbackSourceTransform.position.x < 0 ? -1 : 1;
                directionMultiplier.y = entity.transform.position.y - knockbackComponent.knockbackSourceTransform.position.y < 0 ? -1 : 1; break;
            case DirectionBase.sourceEntityFacingDirection:
                directionMultiplier.x = Mathf.Abs(knockbackComponent.knockbackSourceTransform.rotation.y) < epsilon ? -1 : 1;
                directionMultiplier.y = Mathf.Abs(knockbackComponent.knockbackSourceTransform.rotation.y) < epsilon ? -1 : 1; break;
            case DirectionBase.Absolute:
                directionMultiplier = Vector2.one; break;
            case DirectionBase.positionRelativeDirection:
                directionMultiplier = entity.transform.position - knockbackComponent.knockbackSourceTransform.position; break;
            default:
                break;
        }

        if (knockbackComponent.pertainedCombatAbility.canBeParried)
        {
            if (isParrying)
            {
                Parried();
                entity.animator.SetTrigger("parried");
                entity.animator.SetInteger("typeIndex", UtilityFunctions.RandomInteger(0, 3));
                entity.SetStatusValues(CurrentStatus.Parried);
                entity.entityMovement.SetVelocityChangeOverTime(knockbackComponent.knockbackDirectionWhenParried.normalized * directionMultiplier, knockbackComponent.knockbackSpeedWhenParried, knockbackComponent.knockbackTimeWhenParried, knockbackComponent.easeFunctionWhenParried, true, false);
                // entity.entityMovement.SetVelocityZ(knockbackComponent.orthogonalVelocityWhenParried);
                sourceEntity.SetStatusValues(CurrentStatus.wasParried);

                if (!knockbackComponent.pertainedCombatAbility.stanceWhenParried)
                {
                    sourceEntity.animator.SetTrigger("wasParried");
                    sourceEntity.SetStatusValues(CurrentStatus.wasParried);
                    sourceEntity.entityCombat.ChangeToKnockbackState(knockbackComponent.counterKnockbackTimeWhenParried);

                    Vector2 counterDirectionMultiplier = Vector2.zero;
                    switch (knockbackComponent.directionBase)
                    {
                        case DirectionBase.Position:
                            counterDirectionMultiplier.x = entity.transform.position.x - knockbackComponent.knockbackSourceTransform.position.x < 0 ? -1 : 1;
                            counterDirectionMultiplier.y = entity.transform.position.y - knockbackComponent.knockbackSourceTransform.position.y < 0 ? -1 : 1; break;
                        case DirectionBase.sourceEntityFacingDirection:
                            counterDirectionMultiplier.x = Mathf.Abs(knockbackComponent.knockbackSourceTransform.rotation.y) < epsilon ? -1 : 1;
                            counterDirectionMultiplier.y = Mathf.Abs(knockbackComponent.knockbackSourceTransform.rotation.y) < epsilon ? -1 : 1; break;
                        case DirectionBase.Absolute:
                            counterDirectionMultiplier = Vector2.one; break;
                        case DirectionBase.positionRelativeDirection:
                            counterDirectionMultiplier = entity.transform.position - knockbackComponent.knockbackSourceTransform.position; break;
                        default:
                            break;
                    }
                    sourceEntity.entityMovement.SetVelocityChangeOverTime(knockbackComponent.counterKnockbackDirectionWhenParried.normalized * counterDirectionMultiplier, knockbackComponent.counterKnockbackSpeedWhenParried, knockbackComponent.counterKnockbackTimeWhenParried, knockbackComponent.counterEaseFunctionWhenParried, true, false);
                    // sourceEntity.entityMovement.SetVelocityZ(knockbackComponent.counterOrthogonalVelocityWhenParried);
                }
            }
            else
            {
                if (knockbackComponent.pertainedCombatAbility.canBeBlocked)
                {
                    if (isParrying || isBlocking)
                    {
                        entity.SetStatusValues(CurrentStatus.Blocked);
                        entity.animator.SetTrigger("blocked");
                        entity.entityMovement.SetVelocityChangeOverTime(knockbackComponent.knockbackDirectionWhenBlocked.normalized * directionMultiplier, knockbackComponent.knockbackSpeedWhenBlocked, knockbackComponent.knockbackTimeWhenBlocked, knockbackComponent.easeFunctionWhenBlocked, true, false);
                    }
                    else if (stanceLevel < knockbackComponent.pertainedCombatAbility.threatLevel)
                    {
                        entity.SetStatusValues(CurrentStatus.Knockback);
                        entity.animator.SetBool("airborne", knockbackComponent.airborne || !entity.entityDetection.isGrounded);

                        entity.entityMovement.SetVelocity(knockbackComponent.knockbackDirection.normalized * directionMultiplier * knockbackComponent.knockbackSpeed);
                        entity.entityMovement.SetVelocityZ(knockbackComponent.orthogonalVelocity);

                        entity.entityCombat.ChangeToKnockbackState(knockbackComponent.knockbackTime);
                    }
                }
                else if (stanceLevel < knockbackComponent.pertainedCombatAbility.threatLevel)
                {
                    entity.SetStatusValues(CurrentStatus.Knockback);
                    entity.animator.SetBool("airborne", knockbackComponent.airborne || !entity.entityDetection.isGrounded);

                    entity.entityMovement.SetVelocity(knockbackComponent.knockbackDirection.normalized * directionMultiplier * knockbackComponent.knockbackSpeed);
                    entity.entityMovement.SetVelocityZ(knockbackComponent.orthogonalVelocity);

                    entity.entityCombat.ChangeToKnockbackState(knockbackComponent.knockbackTime);
                }
            }
        }
        else if (knockbackComponent.pertainedCombatAbility.canBeBlocked)
        {
            if (isParrying || isBlocking)
            {
                entity.SetStatusValues(CurrentStatus.Blocked);
                entity.animator.SetTrigger("blocked");
                entity.entityMovement.SetVelocityChangeOverTime(knockbackComponent.knockbackDirectionWhenBlocked.normalized * directionMultiplier, knockbackComponent.knockbackSpeedWhenBlocked, knockbackComponent.knockbackTimeWhenBlocked, knockbackComponent.easeFunctionWhenBlocked, true, false);
            }
            else if (stanceLevel < knockbackComponent.pertainedCombatAbility.threatLevel)
            {
                entity.SetStatusValues(CurrentStatus.Knockback);
                entity.animator.SetBool("airborne", knockbackComponent.airborne || !entity.entityDetection.isGrounded);

                entity.entityMovement.SetVelocity(knockbackComponent.knockbackDirection.normalized * directionMultiplier * knockbackComponent.knockbackSpeed);
                entity.entityMovement.SetVelocityZ(knockbackComponent.orthogonalVelocity);

                entity.entityCombat.ChangeToKnockbackState(knockbackComponent.knockbackTime);
            }
        }
        else if (stanceLevel < knockbackComponent.pertainedCombatAbility.threatLevel)
        {
            entity.SetStatusValues(CurrentStatus.Knockback);
            entity.animator.SetBool("airborne", knockbackComponent.airborne || !entity.entityDetection.isGrounded);

            entity.entityMovement.SetVelocity(knockbackComponent.knockbackDirection.normalized * directionMultiplier * knockbackComponent.knockbackSpeed);
            entity.entityMovement.SetVelocityZ(knockbackComponent.orthogonalVelocity);

            entity.entityCombat.ChangeToKnockbackState(knockbackComponent.knockbackTime);
        }
    }

    protected abstract void ChangeToKnockbackState(float knockbackTime);
    public abstract bool IsParrying(Entity sourceEntity, OverlapCollider[] overlapColliders);
    public abstract bool IsBlocking(Entity sourceEntity, OverlapCollider[] overlapColliders);

    public bool CheckWithinAngle(Vector2 baseVector, Vector2 targetVector, float counterClockwiseAngle, float clockwiseAngle)
    {
        float angleBetweenVectors = -Vector2.SignedAngle(baseVector, targetVector);
        return -counterClockwiseAngle <= angleBetweenVectors && angleBetweenVectors < clockwiseAngle;
    }

    protected void Parried()
    {
        BlockParry[] blockParryPrefabs = entity.orthogonalRigidbody.GetComponentsInChildren<BlockParry>();

        foreach (BlockParry blockParryPrefab in blockParryPrefabs)
        {
            blockParryPrefab.Parried();
        }
    }

    public void DisableBlockParryPrefabs()
    {
        BlockParry[] blockParryPrefabs = entity.orthogonalRigidbody.GetComponentsInChildren<BlockParry>();

        foreach (BlockParry blockParryPrefab in blockParryPrefabs)
        {
            blockParryPrefab.gameObject.SetActive(false);
        }
    }
}
