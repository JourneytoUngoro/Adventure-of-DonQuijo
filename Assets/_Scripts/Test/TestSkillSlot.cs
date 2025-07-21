//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TestSkillSlot : MonoBehaviour
//{
//    public Player player;

//    void Start()
//    {
//        if (player == null) { player = FindAnyObjectByType<Player>(); }
//    }

//    void Update()
//    {
//        if (Manager.Instance.inputHandler.rangedAttackInputPressed)
//        {
//            player.playerStateMachine.ChangeState(player.rangedAttackState);
//            Debug.Log($"PlayerRangedAttackState pressed, changed, cool down time is {player.rangedAttackState.abilityCoolDownTimer.GetLeftTime()}");

//        }
//        else if (Manager.Instance.inputHandler.chargedAttackInputPressed)
//        {
//            player.playerStateMachine.ChangeState(player.chargeAttackState);
//            Debug.Log($"PlayerChargeAttackState pressed, changed, cool down time is {player.chargeAttackState.abilityCoolDownTimer.GetLeftTime()}");

//        }
//        else if ( Manager.Instance.inputHandler.whirlwindInputPressed)
//        {
//            player.playerStateMachine.ChangeState(player.whirlwindState);
//            Debug.Log($"PlayerWhirlwindState pressed, changed, cool down time is {player.whirlwindState.abilityCoolDownTimer.GetLeftTime()}");

//        }



//        //if (player.playerStateMachine.currentState.GetType() == typeof(PlayerRangedAttackState))
//        //{
//        //    UseSkillQ();
//        //}
//        //else if (player.playerStateMachine.currentState.GetType() == typeof(PlayerChargeAttackState))
//        //{
//        //    UseSkillW();
//        //}
//        //else if (player.playerStateMachine.currentState.GetType() == typeof(PlayerWhirlwindState))
//        //{
//        //    UseSkillW();
//        //}

//    }

//    public void UseSkillQ()
//    {

//    }

//    public void UseSkillW()
//    {

//    }

//    public void UseSkillE()
//    {

//    }


//}
