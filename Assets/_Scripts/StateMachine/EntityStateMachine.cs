using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStateMachine
{
    public EntityState entityCurrentState { get; protected set; }
    public EntityState entityPrevState { get; protected set; }
    public EntityState entityNextState { get; protected set; }

    public virtual void Initialize(EntityState startingState)
    {
        entityCurrentState = startingState;
        entityCurrentState.Enter();
    }

    public virtual void ChangeState(EntityState nextState)
    {
        entityNextState = nextState;
        entityCurrentState.Exit();
        entityPrevState = entityCurrentState;
        entityCurrentState = nextState;

        if (entityCurrentState.entity.printStateChange)
        {
            Debug.Log($"State changed from {entityPrevState} to {entityCurrentState}");
        }
    }
}
