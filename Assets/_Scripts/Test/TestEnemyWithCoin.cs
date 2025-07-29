using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyWithCoin : MonoBehaviour
{
    public Transform enemy;

    public void Test_OnEnemyDeath()
    {
        Manager.Instance.stageManager.OnEnemyDeath(enemy);
    }


}