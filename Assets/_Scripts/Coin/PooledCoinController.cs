using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledCoinController : MonoBehaviour
{
    [SerializeField] public GameObject coinPrefab;
    private string coinPrefabName;

    private List<CoinObject> activatedCoinObjects;
    private List<CoinObject> unactivatedCoinObjects;

    private void Awake()
    {
        coinPrefabName = coinPrefab.name;
        activatedCoinObjects = new List<CoinObject>();
        unactivatedCoinObjects = new List<CoinObject>();
    }

    private void Update()
    {
        // coinObject in the Scene disappears after n seconds
        if (activatedCoinObjects != null && activatedCoinObjects.Count > 0)
        {
            for (int i = activatedCoinObjects.Count - 1; i >= 0; i--)
            {
                activatedCoinObjects[i].timer.Tick();
            }
        }

    }

    private void LateUpdate()
    {
        if (unactivatedCoinObjects != null && unactivatedCoinObjects.Count > 0)
        {
            for (int i = 0; i < unactivatedCoinObjects.Count; i++)
            {
                CoinObject removeObject = unactivatedCoinObjects[i];
                unactivatedCoinObjects.Remove(removeObject);
                activatedCoinObjects.Remove(removeObject);
            }
        }
    }

    /// <summary>
    /// Spawns coins at the enemy's death position.
    /// </summary>
    /// <param name="enemy">Transform of the defeated enemy. </param>
    public void DropCoinsFromEnemy(Transform enemy)
    {
        GameObject obj = Manager.Instance.objectPoolingManager.GetGameObject(coinPrefabName);

        // set timer
        SetTimer(obj);

        // set drop effect
        Vector3 startPos = enemy.parent.parent.GetComponentInChildren<SpriteRenderer>().bounds.center;
        Vector3 endPos = enemy.parent.GetComponentInChildren<EnemyDetection>().currentProjectedPosition;

        // drop coin in the scene
        obj.GetComponent<DropEffect>().PlayDropEffect(startPos, endPos);
    }

    private void SetTimer(GameObject obj, float time = -1f)
    {
        CoinObject coinObject = obj.GetComponent<CoinObject>();

        if (time < 0f) coinObject.SetTimer();
        else coinObject.SetTimer(time);
        coinObject.timer.StartSingleUseTimer();
        coinObject.timer.timerAction += () => {
            if (obj.activeSelf)
            {
                coinObject.pooledObject.ReleaseObject();
                RequestRemoveActivatedCoinObject(coinObject);
            }
        };

        activatedCoinObjects.Add(coinObject);
        coinObject.onRelease += RequestRemoveActivatedCoinObject;
    }

    public void RequestRemoveActivatedCoinObject(CoinObject coinObject)
    {
        unactivatedCoinObjects.Add(coinObject);
    }
}
