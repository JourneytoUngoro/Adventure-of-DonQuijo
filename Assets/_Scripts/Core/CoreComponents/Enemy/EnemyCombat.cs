using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [field: SerializeField] public CombatAbilityWithColliders wideRangeAttack { get; private set; }
    [field: SerializeField] public CombatAbilityWithColliders block { get; private set; }
    [field: SerializeField] public CombatAbilityWithColliders parry { get; private set; }
    public int currentBlockStack { get; set; }
    public int currentParryStack { get; set; }

    private Timer blockCoolDownTimer;
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

    protected override void Start()
    {
        base.Start();

        // TODO: Script Order Problem
        EnemyData enemyData = entity.entityData as EnemyData;
        blockCoolDownTimer = new Timer(enemyData.blockCoolDownTime);
        blockCoolDownTimer.timerAction += () => { currentBlockStack = Mathf.Clamp(currentBlockStack + 1, 0, enemyData.maxBlockableCount); };
        blockCoolDownTimer.StartMultiUseTimer();
    }

    protected virtual void Update()
    {
        blockCoolDownTimer.Tick();
    }

    public override bool IsBlocking(Entity sourceEntity, OverlapCollider[] overlapColliders)
    {
        bool blocked = base.IsBlocking(sourceEntity, overlapColliders);

        if (blocked)
        {
            currentParryGauge += enemy.enemyData.parryGaugeWhenBlocked;

            if (currentParryGauge >= 1.0f)
            {
                currentParryGauge -= 1.0f;
                currentParryStack = Mathf.Clamp(currentParryStack + 1, 0, enemy.enemyData.maxParryableCount);
            }
        }

        return blocked;
    }

    public override void WhenGotHit()
    {
        currentParryStack = Mathf.Clamp(currentParryStack + 1, 0, enemy.enemyData.maxParryableCount);
        Manager.Instance.soundManager.PlaySoundFXClip("lightAttack1SFX", enemy.transform);
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
}
