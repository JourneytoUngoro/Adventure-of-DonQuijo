using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyWithCoin : MonoBehaviour
{
    public void DropCoinOnEnemyDeath()
    {
        Manager.Instance.objectPoolingManager.gameObject.GetComponentInChildren<PooledCoinController>().DropCoinsFromEnemy(transform);
    }

    public void DropMemoryFragmentOnEnemyDeath()
    {
        Manager.Instance.objectPoolingManager.gameObject.GetComponentInChildren<PooledMemoryFragmentController>().DropMemoryFragmentFromEnemy(transform);
    }
}
