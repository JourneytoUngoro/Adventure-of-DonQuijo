using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledCoinController : MonoBehaviour
{
    [SerializeField] public GameObject coinPrefab;

    private string coinPrefabName;

    private void Start()
    {
        coinPrefabName = coinPrefab.name;
    }

    /// <summary>
    /// Spawns coins at the enemy's death position.
    /// </summary>
    /// <param name="enemy">Transform of the defeated enemy. </param>
    public void DropCoinsFromEnemy(Transform enemy)
    {
        GameObject coinObject = Manager.Instance.objectPoolingManager.GetGameObject(coinPrefabName);

        Vector3 startPos = enemy.GetComponentInChildren<SpriteRenderer>().bounds.center;
        Vector3 endPos = enemy.GetComponentInChildren<Detection>().currentProjectedPosition;

        coinObject.GetComponent<DropEffect>().PlayDropEffect(startPos, endPos);
    }

}
