using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Combat : CoreComponent
{
    [field: SerializeField] public LayerMask whatIsDamageable { get; protected set; }
    public List<Collider2D> damagedTargets { get; private set; }
    public int stanceLevel { get; protected set; }

    private ContactFilter2D contactFilter;

    protected override void Awake()
    {
        base.Awake();

        contactFilter.SetLayerMask(LayerMask.GetMask("ParryLayer"));
        contactFilter.useLayerMask = true;
    }

    public virtual void DoAttack()
    {

    }

    public void GetMovement(MovementComponent movementComponent)
    {
        entity.entityMovement.SetVelocityChangeOverTime(movementComponent.planeDirection.normalized * movementComponent.planeVelocity, movementComponent.moveTime, movementComponent.easeFunction, movementComponent.slowDown, false);
        entity.entityMovement.SetVelocityZ(movementComponent.orthogonalVelocity);
    }

    public virtual void GetDamage(DamageComponent damageComponent, OverlapCollider[] overlapColliders)
    {
        Entity sourceEntity = damageComponent.pertainedCombatAbility.sourceEntity;
        bool isParrying = damageComponent.pertainedCombatAbility.canBeParried ? IsParrying(sourceEntity, overlapColliders) : false;
        bool isBlocking = damageComponent.pertainedCombatAbility.canBeBlocked ? IsParrying(sourceEntity, overlapColliders) : false;

        GetHealthDamage(damageComponent, isParrying, isBlocking);
        GetPostureDamage(damageComponent, isParrying, isBlocking);
    }

    public virtual void GetHealthDamage(DamageComponent damageComponent, bool isParrying, bool isBlocking)
    {
        Entity sourceEntity = damageComponent.pertainedCombatAbility.sourceEntity;

        float healthDamage = 0;

        for (int key = 0; key <= entity.entityLevel; key++)
        {
            healthDamage = damageComponent.healthDamageIncreaseByLevel.Evaluate(key);
        }
        
        if (damageComponent.pertainedCombatAbility.canBeParried)
        {
            if (isParrying)
            {
                Parried();
                entity.animator.SetTrigger("parried");
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
                        entity.animator.SetTrigger("shielded");
                        entity.entityStats.health.DecreaseCurrentValue(healthDamage * (1.0f - damageComponent.healthDamageShieldRate));
                    }
                    else
                    {
                        entity.SetStatusValues(CurrentStatus.HealthDamage);
                        entity.animator.SetTrigger("gotHit");
                        entity.entityStats.health.DecreaseCurrentValue(healthDamage);
                    }
                }
                else
                {
                    entity.SetStatusValues(CurrentStatus.HealthDamage);
                    entity.animator.SetTrigger("gotHit");
                    entity.entityStats.health.DecreaseCurrentValue(healthDamage);
                }
            }
        }
        else if (damageComponent.pertainedCombatAbility.canBeBlocked)
        {
            if (isParrying || isBlocking)
            {
                entity.SetStatusValues(CurrentStatus.Blocked);
                entity.animator.SetTrigger("shielded");
                entity.entityStats.health.DecreaseCurrentValue(healthDamage * (1.0f - damageComponent.healthDamageShieldRate));
            }
            else
            {
                entity.SetStatusValues(CurrentStatus.GotHit);
                entity.animator.SetTrigger("gotHit");
                entity.entityStats.health.DecreaseCurrentValue(healthDamage);
            }
        }
        else
        {
            entity.SetStatusValues(CurrentStatus.GotHit);
            entity.animator.SetTrigger("gotHit");
            entity.entityStats.health.DecreaseCurrentValue(healthDamage);
        }
    }

    public virtual void GetPostureDamage(DamageComponent damageComponent, bool isParrying, bool isShielding)
    {
        Entity sourceEntity = damageComponent.pertainedCombatAbility.sourceEntity;

        float postureDamage = 0;

        for (int key = 0; key <= entity.entityLevel; key++)
        {
            postureDamage = damageComponent.healthDamageIncreaseByLevel.Evaluate(key);
        }

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

    public virtual void GetKnockback(KnockbackComponent knockbackComponent, OverlapCollider[] overlapColliders)
    {
        entity.SetStatusValues(CurrentStatus.Knockback);
        Entity sourceEntity = knockbackComponent.pertainedCombatAbility.sourceEntity;
        bool isParrying = knockbackComponent.pertainedCombatAbility.canBeParried ? IsParrying(sourceEntity, overlapColliders) : false;
        bool isBlocking = knockbackComponent.pertainedCombatAbility.canBeBlocked ? IsParrying(sourceEntity, overlapColliders) : false;

        Vector2 direction = Vector2.zero;

        switch (knockbackComponent.directionBase)
        {
            case DirectionBase.Position:
                direction.x = entity.transform.position.x - knockbackComponent.knockbackSourceTransform.position.x < 0 ? -1 : 1;
                direction.y = entity.transform.position.y - knockbackComponent.knockbackSourceTransform.position.y < 0 ? -1 : 1; break;
            case DirectionBase.sourceEntityFacingDirection:
                direction.x = Mathf.Abs(knockbackComponent.knockbackSourceTransform.rotation.y) < epsilon ? -1 : 1;
                direction.y = Mathf.Abs(knockbackComponent.knockbackSourceTransform.rotation.y) < epsilon ? -1 : 1; break;
            case DirectionBase.Absolute:
                direction = Vector2.one; break;
            case DirectionBase.positionRelative:
                direction = entity.transform.position - knockbackComponent.knockbackSourceTransform.position; break;
            default:
                break;
        }

        if (knockbackComponent.pertainedCombatAbility.canBeParried)
        {
            if (isParrying)
            {
                Parried();
                entity.animator.SetTrigger("parried");
                entity.SetStatusValues(CurrentStatus.Parried);
                entity.entityMovement.SetVelocityChangeOverTime(knockbackComponent.knockbackDirectionWhenParried.normalized * direction, knockbackComponent.knockbackSpeedWhenParried, knockbackComponent.easeFunctionWhenParried, true, false);
                entity.entityMovement.SetVelocityZ(knockbackComponent.orthogonalVelocityWhenParried);
                sourceEntity.SetStatusValues(CurrentStatus.wasParried);

                if (!knockbackComponent.pertainedCombatAbility.stanceWhenParried)
                {
                    sourceEntity.animator.SetTrigger("wasParried");
                    sourceEntity.entityCombat.ChangeToKnockbackState(knockbackComponent.knockbackTimeWhenParried);

                    sourceEntity.entityMovement.SetVelocityChangeOverTime(knockbackComponent.counterKnockbackDirectionWhenParried.normalized.x * direction, knockbackComponent.counterKnockbackSpeedWhenParried, knockbackComponent.counterEaseFunctionWhenParried, true, false);
                    sourceEntity.entityMovement.SetVelocityZ(knockbackComponent.counterOrthogonalVelocityWhenParried);
                }
            }
            else
            {
                if (knockbackComponent.pertainedCombatAbility.canBeBlocked)
                {
                    if (isParrying || isBlocking)
                    {
                        entity.SetStatusValues(CurrentStatus.Blocked);
                        entity.animator.SetTrigger("shielded");
                        entity.entityMovement.SetVelocityChangeOverTime(knockbackComponent.knockbackDirectionWhenBlocked.normalized * direction, knockbackComponent.knockbackSpeedWhenBlocked, knockbackComponent.easeFunctionWhenBlocked, true, false);
                        entity.entityMovement.SetVelocityZ(knockbackComponent.orthogonalVelocityWhenBlocked);
                    }
                    else
                    {
                        entity.SetStatusValues(CurrentStatus.HealthDamage);
                        entity.animator.SetTrigger("gotHit");
                        entity.entityMovement.SetVelocityChangeOverTime(knockbackComponent.knockbackDirection.normalized * direction, knockbackComponent.knockbackSpeed, knockbackComponent.easeFunctionWhenBlocked, true, false);
                        entity.entityMovement.SetVelocityZ(knockbackComponent.orthogonalVelocity);
                    }
                }
                else
                {
                    entity.SetStatusValues(CurrentStatus.HealthDamage);
                    entity.animator.SetTrigger("gotHit");
                    entity.entityMovement.SetVelocityChangeOverTime(knockbackComponent.knockbackDirection.normalized * direction, knockbackComponent.knockbackSpeed, knockbackComponent.easeFunctionWhenBlocked, true, false);
                    entity.entityMovement.SetVelocityZ(knockbackComponent.orthogonalVelocity);
                }
            }
        }
        else if (knockbackComponent.pertainedCombatAbility.canBeBlocked)
        {
            if (isParrying || isBlocking)
            {
                entity.SetStatusValues(CurrentStatus.Blocked);
                entity.animator.SetTrigger("shielded");
                entity.entityMovement.SetVelocityChangeOverTime(knockbackComponent.knockbackDirectionWhenBlocked.normalized * direction, knockbackComponent.knockbackSpeedWhenBlocked, knockbackComponent.easeFunctionWhenBlocked, true, false);
                entity.entityMovement.SetVelocityZ(knockbackComponent.orthogonalVelocityWhenBlocked);
            }
            else
            {
                entity.SetStatusValues(CurrentStatus.GotHit);
                entity.animator.SetTrigger("gotHit");
                entity.entityMovement.SetVelocityChangeOverTime(knockbackComponent.knockbackDirection.normalized * direction, knockbackComponent.knockbackSpeed, knockbackComponent.easeFunction, true, false);
                entity.entityMovement.SetVelocityZ(knockbackComponent.orthogonalVelocity);
            }
        }
        else
        {
            entity.SetStatusValues(CurrentStatus.GotHit);
            entity.animator.SetTrigger("gotHit");
            entity.entityMovement.SetVelocityChangeOverTime(knockbackComponent.knockbackDirection.normalized * direction, knockbackComponent.knockbackSpeed, knockbackComponent.easeFunction, true, false);
            entity.entityMovement.SetVelocityZ(knockbackComponent.orthogonalVelocity);
        }
    }

    protected abstract void ChangeToKnockbackState(float knockbackTime);

    public bool IsParrying(Entity sourceEntity, OverlapCollider[] overlapColliders)
    {
        bool isParrying = false;

        if (sourceEntity != null)
        {
            List<Collider2D> parryColliders = new List<Collider2D>();
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(LayerMask.GetMask("ParryLayer"));
            contactFilter.useLayerMask = true;

            foreach (OverlapCollider overlapCollider in overlapColliders)
            {
                overlapCollider.overlapCollider.OverlapCollider(contactFilter, parryColliders);
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

    public bool IsBlocking(Entity sourceEntity, OverlapCollider[] overlapColliders)
    {
        bool isBlocking = false;

        if (sourceEntity != null)
        {
            List<Collider2D> parryColliders = new List<Collider2D>();
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(LayerMask.GetMask("BlockLayer"));
            contactFilter.useLayerMask = true;

            foreach (OverlapCollider overlapCollider in overlapColliders)
            {
                overlapCollider.overlapCollider.OverlapCollider(contactFilter, parryColliders);
                BlockParry block = parryColliders.Select(collider => collider.GetComponent<BlockParry>()).Where(blockParry => blockParry.pertainedCombatAbility.sourceEntity.Equals(entity)).FirstOrDefault();

                if (block != null)
                {
                    isBlocking = block.overlapCollider.limitAngle ? CheckWithinAngle(entity.transform.right, sourceEntity.transform.position - entity.transform.position, block.overlapCollider.counterClockwiseAngle, block.overlapCollider.clockwiseAngle) : true;
                }

                if (isBlocking) break;
            }
        }

        return isBlocking;
    }

    public bool CheckWithinAngle(Vector2 baseVector, Vector2 targetVector, float counterClockwiseAngle, float clockwiseAngle)
    {
        float angleBetweenVectors = -Vector2.SignedAngle(baseVector, targetVector);
        return -counterClockwiseAngle <= angleBetweenVectors && angleBetweenVectors < clockwiseAngle;
    }

    protected void Parried()
    {
        BlockParry[] blockParryPrefabs = entity.entityCombat.GetComponentsInChildren<BlockParry>();

        foreach (BlockParry blockParryPrefab in blockParryPrefabs)
        {
            blockParryPrefab.Parried();
        }
    }

    public void ReleaseBlockParryPrefabs()
    {
        BlockParry[] blockParryPrefabs = entity.entityCombat.GetComponentsInChildren<BlockParry>();

        foreach (BlockParry blockParryPrefab in blockParryPrefabs)
        {
            blockParryPrefab.ReleaseObject();
        }
    }
}
