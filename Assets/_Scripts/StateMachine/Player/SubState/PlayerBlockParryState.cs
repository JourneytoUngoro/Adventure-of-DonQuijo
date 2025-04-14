using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlockParryState : PlayerAbilityState
{
    public Timer blockParryCoolDownTimer;

    public PlayerBlockParryState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }
}
