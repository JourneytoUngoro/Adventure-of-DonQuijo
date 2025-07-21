using Sirenix.OdinInspector;
using UnityEngine;

public class ShowInInspector : MonoBehaviour
{
    [ShowInInspector] public float skill1;
    [ShowInInspector] public float skill2;
    [ShowInInspector] public float skill3;

    [ShowInInspector] public bool rangeAttackBool;
    [ShowInInspector] public bool chargeAttackKeyBool;
    [ShowInInspector] public bool whirlwindAttackKeyBool;

    [ShowInInspector] public PlayerState playerState;


    private Player player;
    private PlayerRangedAttackState rangedAttackState;
    private PlayerChargeAttackState chargeAttackState;
    private PlayerWhirlwindState whirlwindState;

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        
        rangedAttackState = player.rangedAttackState;
        chargeAttackState = player.chargeAttackState;
        whirlwindState = player.whirlwindState;
    }

    private void Update()
    {
        skill1 = rangedAttackState.abilityCoolDownTimer.GetLeftTime();
        skill2 = chargeAttackState.abilityCoolDownTimer.GetLeftTime();
        skill3 = whirlwindState.abilityCoolDownTimer.GetLeftTime();

        rangeAttackBool = player.rangedAttackState.available;
        chargeAttackKeyBool = player.chargeAttackState.available;
        whirlwindAttackKeyBool = player.whirlwindState.available;

        playerState = player.playerStateMachine.currentState;

    }
}
