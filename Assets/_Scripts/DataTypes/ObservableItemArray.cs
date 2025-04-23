using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class ObservableItemArray : IObservableArray<Item>
{
    public Item[] items;

    public event Action<Item[]> AnyValueChanged = delegate { };
    public int Count => items.Count(i =>  i!=null);
    public int Length => items.Length;
    public Item this[int index] => items[index];

    public ObservableItemArray(int size = 3, IList<Item> initialList = null)
    {
        items = new Item[size];
        if (initialList != null)
        {
            initialList.Take(size).ToArray().CopyTo(items, 0);
            // Invoke();
        }
    }

    void Invoke() => AnyValueChanged?.Invoke(items);

    public void Swap(int index1, int index2)
    {
        (items[index1], items[index2]) = (items[index2], items[index1]);
        Invoke();
    }

    public void Clear()
    {
        items = new Item[items.Length];
        Invoke();
    }

    // 아이템 목록에 존재하지 않는 아이템을 추가한다 
    public bool TryAdd(Item item)
    {
        for (var i = 0; i < items.Length; i++)
        {
            if (TryAddAt(i, item)) return true;
        }

        return false;
    }

    public bool TryAddAt(int index, Item item)
    {
        if (index < 0 || index >= items.Length) return false;

        if (items[index] != null) return false;

        items[index] = item;
        if (items[index].quantity == 0) items[index].quantity = 1;

        Invoke();

        return true;
    }


    // 아이템 목록에 존재한다면 지운다 
    public bool TryRemove(Item item)
    {
        for (var i = 0; i < items.Length; i++)
        {
            if (TryRemoveAt(i)) return true;
        }

        return false;
    }

    // 해당 인덱스의 아이템을 삭제한다 
    public bool TryRemoveAt(int index)
    {
        if (index < 0 || index >= items.Length) return false;

        if (items[index] == null) return false;

        // TODO : GC가 처리할 수 있도록 확실하게 참조를 없애야 한다 
        items[index] = default;

        Invoke();

        return true;
    }


    // 아이템 목록에 포함됐다면 인덱스를 반환한다
    public int Contains(Item item)
    {
        int index = -1;
        for (var i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].id == item.id) { return i; }
        }

        return index;
    }

    // Contains() -> true :  아이템 수량 증가 
    public bool TryPlusQuantity(int index, Item item, int plusQuantity)
    {
        // 최대 획득 횟수를 초과하지 않았다면
        if (items[index].quantity + plusQuantity <= item.details.maxStack)
        {
            items[index].quantity += plusQuantity;
            Invoke();

            return true;
        }

        return false;
    }

    // Contains() -> true : 아이템 수량 감소
    public bool TryMinusQuantity(int index, Item item, int minusQuantity)
    {        
        // 아이템 목록에 있고, 감소량이 정상 범위 안에 있다면 
        if (items[index].quantity >= minusQuantity )
        {
            items[index].quantity -= minusQuantity;

            // TODO : GC가 처리할 수 있도록 확실하게 참조를 없애야 한다 
            if (items[index].quantity == 0) { items[index] = default; }

            Invoke();

            return true;
        }

        return false;
    }

    public int Quantity(Item item)
    {
        int index = Contains(item);

        if (index != -1) return items[index].quantity;

        return index;
    }


}
