using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyWithCoin : MonoBehaviour
{
    public void Test_OnEnemyDeath()
    {

        Manager.Instance.stageManager.OnEnemyDeath(transform);
    }


}