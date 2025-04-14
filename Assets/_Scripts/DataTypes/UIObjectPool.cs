using System.Collections.Generic;
using UnityEngine;

public class UIObjectPool<T> where T : MonoBehaviour
{
    private readonly Queue<T> pool;
    private  readonly T prefab;
    
    public UIObjectPool(T prefab, int initialSize)
    {
        this.prefab = prefab;

        pool = new Queue<T>();

        for (int i = 0; i < initialSize; i++)
        {
            T obj = Object.Instantiate(prefab);
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
            return obj;
        }
    }

    public void Return(T obj)
    {
        pool.Enqueue(obj);
        Debug.Log("Returned obj");
    }

}
