using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBlockParryState : EnemyAbilityState
{
    private Timer blockParryTimer;

    public EnemyBlockParryState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        available = true;
        blockParryTimer = new Timer(enemy.enemyData.blockParryTime);
        blockParryTimer.timerAction += () => { isAbilityDone = true; };
    }

    public override void AnimationStartTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        blockParryTimer.StopTimer();
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
        enemy.movement.SetVelocity(0.0f, 0.0f);
        
        blockParryTimer.StartSingleUseTimer();

        if (enemy.combat.currentParryStack > 0)
        {
            enemy.combat.currentParryStack -= 1;
            enemy.combat.DoAttack(enemy.combat.parry);
        }
        else
        {
            enemy.combat.currentBlockStack -= 1;
            enemy.combat.DoAttack(enemy.combat.block);
        }
    }

    public override void Exit()
    {
        base.Exit();

        available = true;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            blockParryTimer.Tick();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

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
                        stateMachine.ChangeState(enemy.idleState);
                    }
                }
                else
                {

                }
            }
        }
    }
}
