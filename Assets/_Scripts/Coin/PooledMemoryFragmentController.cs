using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledMemoryFragmentController : MonoBehaviour
{
    [SerializeField] public GameObject memoryFragmentPrefab;

    private string memoryFragmentPrefabName;

    private void Start()
    {
        memoryFragmentPrefabName = memoryFragmentPrefab.name;
    }

    /// <summary>
    /// Spawns memoryFragment at the enemy's death position.
    /// </summary>
    /// <param name="enemy">Transform of the defeated enemy. </param>
    public void DropMemoryFragmentFromEnemy(Transform enemy)
    {
        GameObject memoryFragmentObejct = Manager.Instance.objectPoolingManager.GetGameObject(memoryFragmentPrefabName);

        Vector3 startPos = enemy.GetComponentInChildren<SpriteRenderer>().bounds.center;
        Vector3 endPos = enemy.GetComponentInChildren<Detection>().currentProjectedPosition;

        memoryFragmentObejct.GetComponent<DropEffect>().PlayDropEffect(startPos, endPos);
    }

}

