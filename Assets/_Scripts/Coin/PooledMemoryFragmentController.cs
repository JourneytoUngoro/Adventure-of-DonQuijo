using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledMemoryFragmentController : MonoBehaviour, IDataPersistance
{
    [SerializeField] public GameObject memoryFragmentPrefab;

    private string memoryFragmentPrefabName;

    private List<MemoryFragmentObject> activatedMemoryFragmentObjects;
    private List<MemoryFragmentObject> unactivatedMemoryFragmentObjects;

    private void Awake()
    {
        memoryFragmentPrefabName = memoryFragmentPrefab.name;
        activatedMemoryFragmentObjects = new List<MemoryFragmentObject>();
        unactivatedMemoryFragmentObjects = new List<MemoryFragmentObject>();
    }

    private void Update()
    {
        // memoryFragmentObject in the Scene disappears after n seconds
        if (activatedMemoryFragmentObjects != null && activatedMemoryFragmentObjects.Count > 0)
        {
            for (int i = activatedMemoryFragmentObjects.Count - 1; i >= 0; i--)
            {
                activatedMemoryFragmentObjects[i].timer.Tick();
            }
        }
    }

    private void LateUpdate()
    {
        if (unactivatedMemoryFragmentObjects != null && unactivatedMemoryFragmentObjects.Count > 0)
        {
            for (int i = 0; i < unactivatedMemoryFragmentObjects.Count; i++)
            {
                MemoryFragmentObject removeObject = unactivatedMemoryFragmentObjects[i];
                unactivatedMemoryFragmentObjects.Remove(removeObject);
                activatedMemoryFragmentObjects.Remove(removeObject);
            }
        }
    }

    /// <summary>
    /// Spawns memoryFragment at the enemy's death position.
    /// </summary>
    /// <param name="enemy">Transform of the defeated enemy. </param>
    public void DropMemoryFragmentFromEnemy(Transform enemy)
    {
        GameObject obj = Manager.Instance.objectPoolingManager.GetGameObject(memoryFragmentPrefabName);

        // set timer
        SetTimer(obj);

        // set drop effect
        Vector3 startPos = enemy.parent.parent.GetComponentInChildren<SpriteRenderer>().bounds.center;
        Vector3 endPos = enemy.parent.GetComponentInChildren<EnemyDetection>().currentProjectedPosition;

        // drop mf in Scene
        obj.GetComponent<DropEffect>().PlayDropEffect(startPos, endPos);
    }

    private void SetTimer(GameObject obj, float time = -1f)
    {
        MemoryFragmentObject mfObject = obj.GetComponent<MemoryFragmentObject>();
        if (time < 0f) mfObject.SetTimer();
        else mfObject.SetTimer(time);
        mfObject.timer.StartSingleUseTimer();
        mfObject.timer.timerAction += () => {
            if (obj.activeSelf)
            {
                mfObject.pooledObject.ReleaseObject();
                RequestRemoveActivatedMFObject(mfObject);
            }
        };
        activatedMemoryFragmentObjects.Add(mfObject);
        mfObject.onRelease += RequestRemoveActivatedMFObject;
    }

    public void RequestRemoveActivatedMFObject(MemoryFragmentObject mfObject)
    {
        activatedMemoryFragmentObjects.Remove(mfObject);
    }


    public void LoadData(GameData data)
    {
        MemoryFragmentData mfData = data.memoryFragmentData;

        bool isNew = (mfData.memoryFragmentPositions.Count == 0 || mfData.count == 0);

        if (!isNew)
        {
            foreach (SerializableVector3 pos in mfData.memoryFragmentPositions)
            {
                GameObject obj = Manager.Instance.objectPoolingManager.GetGameObject(memoryFragmentPrefabName);

                obj.transform.position = pos.ToVector3();

                float newLifeTime = obj.GetComponent<MemoryFragmentObject>().mfLifeTime - Random.Range(0.01f, 1.5f);

                SetTimer(obj, newLifeTime);
            }
        }
    }

    public void SaveData(GameData data)
    {
        MemoryFragmentData mfData = new MemoryFragmentData();

        mfData.count = activatedMemoryFragmentObjects.Count;

        foreach (MemoryFragmentObject mf in activatedMemoryFragmentObjects)
        {
            mfData.AddMemoryFragmentPositions(mf.transform.position);
        }

        data.memoryFragmentData = mfData;
    }

}
