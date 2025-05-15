using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : Combat
{
    protected Enemy enemy;
    [field: SerializeField] public LayerMask whatIsChaseTarget { get; protected set; }
    [field: SerializeField] public List<CombatAbilityWithColliders> meleeAttack0 { get; private set; }
    [field: SerializeField] public List<CombatAbilityWithColliders> meleeAttack1 { get; private set; }
    [field: SerializeField] public List<CombatAbilityWithColliders> meleeAttack2 { get; private set; }
    [field: SerializeField] public List<CombatAbilityWithColliders> dodgeAttack { get; private set; }
    [field: SerializeField] public List<CombatAbilityWithColliders> dashAttack { get; private set; }
    // [field: SerializeField] public CombatAbilityWithColliders dashAttack { get; private set; }
    [field: SerializeField] public CombatAbilityWithColliders wideRangeAttack { get; private set; }

    [SerializeField] private Transform blockParryTransform;
    private Timer blockCoolDownTimer;
    private int currentBlockStack;
    private int currentParryStack;
    private float currentParryGauge;

    private Collider2D[] detectedEntities = new Collider2D[maxDetectionCount];
    private ContactFilter2D contactFilter;

    protected override void Awake()
    {
        base.Awake();

        enemy = entity as Enemy;
        contactFilter.SetLayerMask(whatIsChaseTarget);
        contactFilter.useLayerMask = true;
    }

    protected virtual void Start()
    {
        EnemyData enemyData = entity.entityData as EnemyData;
        blockCoolDownTimer = new Timer(enemyData.blockCoolDownTime);
        blockCoolDownTimer.timerAction += () => { currentBlockStack = Mathf.Clamp(currentBlockStack + 1, 0, enemyData.maxBlockableCount); };
    }

    protected virtual void Update()
    {
        blockCoolDownTimer.Tick();
    }

    public override void GetKnockback(KnockbackComponent knockbackComponent, OverlapCollider[] overlapColliders)
    {
        if (!enemy.enemyStateMachine.currentState.Equals(enemy.stunnedState))
        {
            base.GetKnockback(knockbackComponent, overlapColliders);
        }
    }

    protected override void ChangeToKnockbackState(float knockbackTime)
    {
        enemy.knockbackState.knockbackTimer.ChangeDuration(knockbackTime);
        enemy.enemyStateMachine.ChangeState(enemy.knockbackState);
    }

    public override bool IsParrying(Entity sourceEntity, OverlapCollider[] overlapColliders)
    {
        if (currentParryStack > 0)
        {
            currentParryStack -= 1;

            if ((sourceEntity.transform.position.x - enemy.transform.position.x) * enemy.movement.facingDirection >= 0)
            {
                Debug.Log("Enemy Parried");
                enemy.animator.SetTrigger("parried");
                enemy.enemyStateMachine.ChangeState(enemy.blockParryState);
                return true;
            }
            else return false;
        }
        else return false;
    }

    public override bool IsBlocking(Entity sourceEntity, OverlapCollider[] overlapColliders)
    {
        if (currentBlockStack > 0)
        {
            currentBlockStack -= 1;

            if ((sourceEntity.transform.position.x - enemy.transform.position.x) * enemy.movement.facingDirection >= 0)
            {
                currentParryGauge += enemy.enemyData.parryGaugeWhenBlocked;
                if (currentParryGauge >= 1.0f)
                {
                    currentParryStack = Mathf.Clamp(currentParryStack + 1, 0, enemy.enemyData.maxParryableCount);
                }

                Debug.Log("Enemy Blocked");
                enemy.animator.SetTrigger("blocked");
                enemy.enemyStateMachine.ChangeState(enemy.blockParryState);
                return true;
            }
            else return false;
        }
        else return false;
    }

    public bool IsTargetInRangeOf(CombatAbilityWithColliders combatAbility)
    {
        if (enemy.detection.currentTarget == null) return false;

        bool targetInRange = false;

        Array.Clear(detectedEntities, 0, maxDetectionCount);

        foreach (OverlapCollider overlapCollider in combatAbility.overlapColliders)
        {
            overlapCollider.overlapCollider.OverlapCollider(contactFilter, detectedEntities);
            targetInRange = Array.Exists(detectedEntities, detectedEntity => detectedEntity != null && detectedEntity == enemy.detection.currentTarget.entityCollider);

            if (targetInRange) break;
        }

        return targetInRange;
    }

    public bool IsTargetInRangeOf(List<CombatAbilityWithColliders> combatAbilities)
    {
        if (enemy.detection.currentTarget == null) return false;

        bool targetInRange = false;

        Array.Clear(detectedEntities, 0, maxDetectionCount);

        foreach (CombatAbilityWithColliders combatAbility in combatAbilities)
        {
            foreach (OverlapCollider overlapCollider in combatAbility.overlapColliders)
            {
                overlapCollider.overlapCollider.OverlapCollider(contactFilter, detectedEntities);
                targetInRange = Array.Exists(detectedEntities, detectedEntity => detectedEntity != null && detectedEntity == enemy.detection.currentTarget.entityCollider);

                if (targetInRange) break;
            }
        }

        return targetInRange;
    }

    public void Alerted(Entity sourceEntity, CombatAbility combatAbility)
    {
        if (currentParryStack > 0)
        {

        }
        else if (currentBlockStack > 0)
        {

        }
        
        enemy.enemyStateMachine.ChangeState(enemy.blockParryState);
    } 
}
