using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PooledObject : MonoBehaviour
{
    public IObjectPool<GameObject> objectPool;
    public bool isPooled = true;

    public virtual void ReleaseObject()
    {
        if (isPooled == false)
        {
            gameObject.transform.SetParent(Manager.Instance.objectPoolingManager.transform);
            objectPool.Release(gameObject);
        }
    }
}
