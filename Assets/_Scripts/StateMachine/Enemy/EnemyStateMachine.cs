using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : EntityStateMachine
{
    public EnemyState prevState { get; private set; }
    public EnemyState currentState { get; private set; }    
    public EnemyState nextState { get; private set; }

    public override void Initialize(EntityState startingState)
    {
        currentState = startingState as EnemyState;
        base.Initialize(startingState);
    }

    public override void ChangeState(EntityState nextState)
    {
        entityNextState = nextState;
        this.nextState = nextState as EnemyState;

        Debug.Assert(currentState != null, "Enemy StateMachine CurrentState is null!");

        currentState.Exit();

        entityPrevState = entityCurrentState;
        prevState = currentState;

        entityCurrentState = nextState;
        currentState = nextState as EnemyState;

        Debug.Assert(currentState != null, "Enemy StateMachine CurrentState is null!");

        if (currentState == null) { currentState = prevState; } // 임시로 한 프레임 더 유지해보기

        currentState.Enter();

        if (currentState.enemy.printStateChange)
        {
            Debug.Log($"State changed from {prevState} to {currentState}");
        }
    }
}
