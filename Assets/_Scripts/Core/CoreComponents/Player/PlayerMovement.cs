using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{
    [field: SerializeField] public float jumpSpeed;

    private Player player;

    protected override void Awake()
    {
        base.Awake();

        player = entity as Player;
    }
}
