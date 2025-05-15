using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack1State : EnemyAbilityState
{
    public EnemyMeleeAttack1State(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        available = true;
        abilityCoolDownTimer.ChangeDuration(enemyData.meleeAttack1CoolDown);
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        enemy.combat.DoAttack(enemy.combat.meleeAttack1[index]);
        enemy.combat.damagedTargets.Clear();
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        isAbilityDone = true;
    }

    public override void Enter()
    {
        base.Enter();

        available = false;
        enemy.movement.SetVelocityZero();
        enemy.animator.SetInteger("typeIndex", 1);
    }

    public override void Exit()
    {
        base.Exit();

        abilityCoolDownTimer.StartSingleUseTimer();
        enemy.combat.damagedTargets.Clear();
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
                else
                {
                    // stateMachine.ChangeState(enemy.inAirState);
                }
            }
        }
    }
}
