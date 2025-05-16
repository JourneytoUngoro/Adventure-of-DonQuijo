using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDodgeAttackState : EnemyAbilityState
{
    private bool startDashAttack;

    public EnemyDodgeAttackState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        available = true;
        abilityCoolDownTimer.ChangeDuration(enemyData.dodgeAttackCoolDown);
    }

    public override void AnimationStartTrigger(int index)
    {
        base.AnimationStartTrigger(index);

        enemy.combat.DoAttack(enemy.combat.dodgeAttack[0]);
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        enemy.gameObject.tag = "Idle";
        startDashAttack = true;
        enemy.combat.DoAttack(enemy.combat.dodgeAttack[1]);
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
        enemy.gameObject.tag = "Dodge";
        startDashAttack = false;
        enemy.combat.SetStanceLevel(1);
    }

    public override void Exit()
    {
        base.Exit();

        abilityCoolDownTimer.StartSingleUseTimer();
        enemy.gameObject.tag = "Idle";
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
                else
                {
                    // stateMachine.ChangeState(enemy.inAirState);
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

                enemy.combat.DoAttack(enemy.combat.dashAttack[1], false);
            }
        }
    }
}
