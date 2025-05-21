using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeadState : EnemyState
{
    public EnemyDeadState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        enemy.gameObject.SetActive(false);
    }

    public override void Enter()
    {
        base.Enter();

        enemy.gameObject.tag = "Invinsible";
        enemy.stateMachineToAnimator.state = this;
    }
}
