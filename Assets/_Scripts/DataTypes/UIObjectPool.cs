using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UIObjectPool<T> where T : MonoBehaviour
{
    private readonly Queue<T> pool;
    private  readonly T prefab;
    private readonly int limitedCount;

    Transform parent;

    public  int Count { get =>  pool.Count; }
    
    public UIObjectPool(T prefab, int initialSize, Transform parent)
    {
        this.prefab = prefab;
        this.limitedCount = initialSize;
        this.parent = parent;

        pool = new Queue<T>();

        for (int i = 0; i < initialSize; i++)
        {
            T obj = Object.Instantiate(prefab);
            obj.transform.SetParent(parent);
            pool.Enqueue(obj);
        }
    }

    public T Get()
    {
        if (pool.Count > 0)
        {
            T obj = pool.Dequeue();
            return obj;
        }
        else
        {
            T obj = Object.Instantiate(prefab);
            obj.transform.SetParent(parent);
            return obj;
        }
    }

    public void Return(T obj)
    {
        if (pool.Count <= limitedCount)
        {
            pool.Enqueue(obj);
            obj.transform.SetParent(parent);
        }
        else
        {
            // T가 Unity 오브젝트일 경우에만 Destroy
            if (obj is Component comp)
            {
                UnityEngine.Object.Destroy(comp.gameObject);
            }
        }
    }

}
