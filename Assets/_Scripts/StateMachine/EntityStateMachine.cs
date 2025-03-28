using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityStateMachine
{
    public EntityState entityCurrentState { get; protected set; }
    public EntityState entityPrevState { get; protected set; }
    public EntityState entityNextState { get; protected set; }

    public virtual void Initialize(EntityState startingState)
    {
        entityCurrentState = startingState;
        entityCurrentState.Enter();
    }

    public abstract void ChangeState(EntityState nextState);
}
