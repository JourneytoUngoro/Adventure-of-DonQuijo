using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : EntityState
{
    public Enemy enemy { get; private set; }

    public EnemyState(Entity entity, string animBoolName) : base(entity, animBoolName)
    {
    }
}
