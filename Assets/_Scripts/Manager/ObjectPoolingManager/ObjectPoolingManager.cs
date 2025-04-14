using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

[System.Serializable]
public class ObjectInfo
{
    [HideInInspector] public string objectName;
    public GameObject prefab;
    public int defaultCapacity;
}

public class ObjectPoolingManager : MonoBehaviour
{
    [field: SerializeField] public List<ObjectInfo> objectInfos { get; private set; }

    private string objectName;

    private Dictionary<string, IObjectPool<GameObject>> objectPoolDictionary = new Dictionary<string, IObjectPool<GameObject>>();

    private void Awake()
    {
        Initialize();
    }


    private void Initialize()
    {
        for (int objectIndex = 0; objectIndex < objectInfos.Count; objectIndex++)
        {
            IObjectPool<GameObject> objectPool = new ObjectPool<GameObject>(CreatePooledObject, OnTakeFromPool, OnReturnToPool, OnDestroyPooledObject, true, objectInfos[objectIndex].defaultCapacity);

            if (objectPoolDictionary.ContainsKey(objectInfos[objectIndex].prefab.name))
            {
                Debug.LogWarning($"{objectInfos[objectIndex].prefab.name} is already pooled.");
                continue;
            }

            objectPoolDictionary.Add(objectInfos[objectIndex].prefab.name, objectPool);

            for (int objectCount = 0; objectCount < objectInfos[objectIndex].defaultCapacity; objectCount++)
            {
                objectName = objectInfos[objectIndex].prefab.name;
                CreatePooledObject().GetComponent<PooledObject>().ReleaseObject();
            }
        }
    }

    private GameObject CreatePooledObject()
    {
        GameObject pooledObject = Instantiate(objectInfos.FirstOrDefault(objectInfo => objectInfo.prefab.name.Equals(objectName)).prefab);
        pooledObject.GetComponent<PooledObject>().objectPool = objectPoolDictionary[objectName];
        pooledObject.transform.parent = transform;
        return pooledObject;
    }

    private void OnTakeFromPool(GameObject pooledObject)
    {
        pooledObject.GetComponent<PooledObject>().isPooled = false;
        pooledObject.SetActive(true);
    }

    private void OnReturnToPool(GameObject pooledObject)
    {
        pooledObject.GetComponent<PooledObject>().isPooled = true;
        pooledObject.SetActive(false);
    }

    private void OnDestroyPooledObject(GameObject pooledObject)
    {
        Destroy(pooledObject);
    }

    public GameObject GetGameObject(string objectName)
    {
        this.objectName = objectName;

        if (objectPoolDictionary.ContainsKey(objectName) == false)
        {
            Debug.LogError($"{objectName} is not assigned to pool.");
            return null;
        }

        return objectPoolDictionary[objectName].Get();
    }

    public void ReleaseGameObject(GameObject pooledObject)
    {
        pooledObject.GetComponent<PooledObject>().ReleaseObject();
    }
}