using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SkillState
{
    ChargeAttack,
    RangedAttack,
    Whirlwind
}

public class SkillSlotUI : MonoBehaviour
{
    public GameObject slotUI;
    private Image coolDownImage;

    public SkillState skillState;

    private PlayerAbilityState abilitySkillState;
    private Timer coolDownTimer;
    private float coolTime;

    private bool hasStarted;

    private void Start()
    {
        #region Test
        TestTimer();
        #endregion

        // GetSkillStateByState();
        // coolDownTimer = abilitySkillState.abilityCoolDownTimer;
        // coolTime = coolDownTimer.duration;
        // abilitySkillState.

        coolDownImage = transform.Find("Timer Sprite").GetComponent<Image>();
        coolDownImage.fillAmount = 0;

        hasStarted = false;


        Debug.Assert(coolDownImage != null, "cool down image is null!");
    }

    private void GetSkillStateByState()
    {
        abilitySkillState = skillState switch
        {
            SkillState.ChargeAttack => FindAnyObjectByType<Player>().abilityStates.Find(x => x.GetType().Name == "PlayerWhirlwindState"),
            SkillState.RangedAttack => FindAnyObjectByType<Player>().abilityStates.Find(x => x.GetType().Name == "PlayerChargeAttackState"),
            SkillState.Whirlwind => FindAnyObjectByType<Player>().abilityStates.Find(x => x.GetType().Name == "PlayerRangedAttackState"),
            _ => null
        }; 
        
        Debug.Assert(abilitySkillState != null, "skill slot is null!");
    }

    private void Update()
    {

        if (coolDownTimer.timerActive && !hasStarted)
        {
            StartSkillCoolDown();
        }

        if (hasStarted)
        {
            float fillAmount = coolDownTimer.GetLeftTime() / coolTime;
            coolDownImage.fillAmount = fillAmount;
        }
        
        if (!coolDownTimer.timerActive && hasStarted)
        {
            FinishSkillCoolDown();
        }
    }

    private void StartSkillCoolDown()
    {
        coolDownImage.fillAmount = 1;
        hasStarted = true;
    }

    private void FinishSkillCoolDown()
    {
        coolDownImage.fillAmount = 0;
        hasStarted = false;
    }

    private void TestTimer()
    {
        coolDownTimer = new Timer(7f);
        coolTime = coolDownTimer.duration;
    }

    public void ExecuteTestTimer()
    {
        coolDownTimer.StartSingleUseTimer();
        Debug.Log($"timer.timerAcitve changed : {coolDownTimer.timerActive}");
    }

}
