using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockParry : MonoBehaviour
{
    public OverlapCollider overlapCollider { get; private set; }
    public CombatAbility pertainedCombatAbility { get; private set; }

    private float parryStartTime;
    private float parryTime;
    private float parryDurationTime;
    private bool changeToBlock = true;

    private bool isParried;

    private void OnEnable()
    {
        parryStartTime = Time.time;
    }

    private void OnDisable()
    {
        isParried = false;
        parryTime = 0.0f;
        parryDurationTime = 0.0f;
        changeToBlock = true;
    }

    private void Update()
    {
        if (!isParried)
        {
            if (Time.time > parryStartTime + parryTime)
            {
                if (changeToBlock)
                {
                    gameObject.layer = LayerMask.NameToLayer("BlockLayer");
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }
        else
        {
            if (Time.time > parryStartTime + parryDurationTime)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void SetParryData(CombatAbility pertainedCombatAbility, float parryTime, float parryDurationTime, bool changeToBlock, OverlapCollider overlapCollider)
    {
        this.pertainedCombatAbility = pertainedCombatAbility;
        this.parryTime = parryTime;
        this.parryDurationTime = parryDurationTime;
        this.changeToBlock = changeToBlock;
        this.overlapCollider = overlapCollider;
        gameObject.layer = LayerMask.NameToLayer("ParryLayer");
        gameObject.SetActive(true);
    }

    public void SetBlockData(CombatAbility pertainedCombatAbility, OverlapCollider overlapCollider)
    {
        this.pertainedCombatAbility = pertainedCombatAbility;
        this.overlapCollider = overlapCollider;
        gameObject.layer = LayerMask.NameToLayer("BlockLayer");
        gameObject.SetActive(true);
    }

    public void Parried()
    {
        isParried = true;
        parryStartTime = Time.time;
    }
}
