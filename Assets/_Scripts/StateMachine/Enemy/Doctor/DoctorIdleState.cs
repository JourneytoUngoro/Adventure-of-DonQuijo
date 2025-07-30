using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorIdleState : EnemyIdleState
{
    private Doctor doctor;

    public DoctorIdleState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        doctor = enemy as Doctor;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!onStateExit)
        {
            if (isTargetInDetectionRange && !enemy.detection.currentTarget.isDead)
            {
                stateMachine.ChangeState(doctor.targetInDetectionRangeState);
            }
        }
    }
}
