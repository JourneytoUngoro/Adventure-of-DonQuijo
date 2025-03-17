using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : EntityStateMachine
{
    public PlayerState currentState { get; private set; }
    public PlayerState prevState { get; private set; }
    public PlayerState nextState { get; private set; }

    public override void Initialize(EntityState startingState)
    {
        currentState = startingState as PlayerState;
        base.Initialize(startingState);
    }

    public override void ChangeState(EntityState nextState)
    {
        this.nextState = nextState as PlayerState;
        base.ChangeState(nextState);
        prevState = currentState;
        currentState = nextState as PlayerState;
        currentState.Enter();
    }
}
