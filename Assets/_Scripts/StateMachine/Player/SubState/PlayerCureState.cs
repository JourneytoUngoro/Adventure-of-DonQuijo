using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCureState : PlayerAbilityState
{
    public PlayerCureState(Player player, string animBoolName) : base(player, animBoolName)
    {
        // 아이템 사용 가능 여부 확인 
        available = false;
    }

    public override void AnimationActionTrigger(int index)
    {
        base.AnimationActionTrigger(index);

        player.stats.health.IncreaseCurrentValue(player.stats.health.maxValue);
    }

    public override void AnimationFinishTrigger(int index)
    {
        base.AnimationFinishTrigger(index);

        isAbilityDone = true;
    }

    public override void Enter()
    {
        base.Enter();

        player.movement.SetVelocityZero();
        player.stateMachineToAnimator.state = this;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        #region Physics Logic
        if (!onStateExit)
        {
            player.movement.SetVelocityZero();
            // player.movement.SetPlaneVelocity(0.0f, 0.0f);
        }
        #endregion
    }
}