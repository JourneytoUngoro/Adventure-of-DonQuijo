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
        SetSkillStateByState();
        coolDownTimer = abilitySkillState.abilityCoolDownTimer;
        coolTime = coolDownTimer.duration;

        coolDownImage = transform.Find("Timer Sprite").GetComponent<Image>();
        coolDownImage.fillAmount = 0; // Top -> Bottom 

        hasStarted = false;
    }

    private void SetSkillStateByState()
    { 
        abilitySkillState = skillState switch
        {
            SkillState.ChargeAttack => FindAnyObjectByType<Player>().abilityStates.Find(x => x.GetType() == typeof(PlayerChargeAttackState)),
            SkillState.RangedAttack => FindAnyObjectByType<Player>().abilityStates.Find(x => x.GetType() == typeof(PlayerRangedAttackState)),
            SkillState.Whirlwind => FindAnyObjectByType<Player>().abilityStates.Find(x => x.GetType() == typeof(PlayerWhirlwindState)),
            _ => null
        }; 
        
        // Debug.Log($"{transform.gameObject.name} slot related to {abilitySkillState} ({skillState.ToString()})");
    }

    private void Update()
    {

        if (coolDownTimer.timerActive && !hasStarted)
        {
            StartSkillCoolDown();
        }

        if (hasStarted) // Skill coolDownTimer is running...
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
        coolDownImage.fillAmount = 1.0f;
        hasStarted = true;
    }

    private void FinishSkillCoolDown()
    {
        coolDownImage.fillAmount = 0f;
        hasStarted = false;
    }
}
