using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDashAttackState : EnemyAbilityState
{
    private bool startDashAttack;

    public EnemyDashAttackState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        available = true;
        abilityCoolDownTimer.ChangeDuration(enemy.enemyData.dashAttackCoolDown);
        // dashTimer = new Timer(enemy.combat.dashAttack[0])
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        startDashAttack = true;
        enemy.combat.DoAttack(enemy.combat.dashAttack[0]);

        if (enemy.combat.DoAttack(enemy.combat.dashAttack[1]).second)
        {
            isAbilityDone = true;
            startDashAttack = false;
            enemy.movement.StopVelocityChangeOverTime();
            enemy.movement.SetVelocityZero();
        }
    }

    public override void Enter()
    {
        base.Enter();

        available = false;
        startDashAttack = false;
        enemy.combat.SetStanceLevel(1);
    }

    public override void Exit()
    {
        base.Exit();

        abilityCoolDownTimer.StartSingleUseTimer();
        enemy.combat.damagedTargets.Clear();
        enemy.movement.StopVelocityChangeOverTime();
        enemy.movement.SetVelocityZero();
        enemy.combat.SetStanceLevel(0);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (isAbilityDone)
            {
                if (isGrounded)
                {
                    if (isTargetInDetectionRange)
                    {
                        stateMachine.ChangeState(enemy.targetInDetectionRangeState);
                    }
                    else
                    {
                        if (enemy.detection.currentTargetLastVelocity.x * facingDirection < 0)
                        {
                            enemy.movement.Flip();
                        }
                        stateMachine.ChangeState(enemy.idleState);
                    }
                }
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!onStateExit)
        {
            if (startDashAttack)
            {
                isAbilityDone = !enemy.movement.coroutineEnabled;

                if (enemy.combat.DoAttack(enemy.combat.dashAttack[1]).second)
                {
                    isAbilityDone = true;
                    enemy.movement.StopVelocityChangeOverTime();
                    enemy.movement.SetVelocityZero();
                }
            }
        }
    }
}
