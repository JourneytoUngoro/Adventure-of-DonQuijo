using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObservableArray<T>
{
    // 외부에서 호출할 수 있어야 한다면, event Action 대신 event를 고려
    event Action<T[]> AnyValueChanged;

    int Count { get; }
    T this[int index] { get; }

    void Swap(int index1, int index2);
    void Clear();
    bool TryAdd(T item);
    bool TryAddAt(int index, T item);
    bool TryRemove(T item);
    bool TryRemoveAt(int index);
}