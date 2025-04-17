using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryModel 
{
    public ObservableItemArray Items { get; private set; }
    InventoryData inventoryData = new InventoryData();
    readonly int capacity;

    public int coins
    {
        get => inventoryData.coins;
        set => inventoryData.coins = value;
    }

    public event Action<Item[]> OnModelChanged
    {
        add => Items.AnyValueChanged += value;
        remove => Items.AnyValueChanged -= value;
    }

    public InventoryModel(IEnumerable<ItemDetails> itemDetails, int capacity)
    {
        this.capacity = capacity;

        coins = 1000; // ================ TODO : 테스트용 지워야 함 ===============


        // inventory, controller의 startingItems
        // item이 생성되어 삽입된다
        Items = new ObservableItemArray(capacity);
        foreach (var details in itemDetails)
        {
            bool success = Items.TryAdd(details.Create(1));
            // Debug.Log(success ? details.label + "생성되어 Model에 등록됨 " : details.label + "생성됐으나 Model에 등록은 실패");
            
        }
    }

    // Inventory -> Controller에 의해 호출된다 
    public void LoadData(InventoryData data)
    {
        inventoryData = data; // inventoryData에는 item[] 배열이 저장되어 있다 
        inventoryData.capacity = capacity;

        //               테스틑용 ===================================
        inventoryData.coins = inventoryData.coins == 0 ? 1000 : inventoryData.coins;

        bool isNew = inventoryData.items == null || inventoryData.items.Length == 0;

        if (isNew)
        {
            inventoryData.items = new Item[capacity];
        }
        else
        {
            for (int i = 0; i < capacity; i++)
            {
                if (Items[i] == null) continue;
                inventoryData.items[i].details = ItemDatabase.GetDetailsById(i);
                Debug.Log(inventoryData.items[i].details.label);
            }
        }

        // 컨트롤러에서 모델을 생성할 때 할당한 게 있는 경우 
        if (isNew && Items.Count != 0)
        {
            for (int i = 0; i < capacity; i++)
            {
                if (Items[i] == null) continue;
                inventoryData.items[i] = Items[i];
            }
        }

        // this.Items : 모델이 가진 ObservableItemArray 변수
        // ~.Items = ObservableItemArray 내부의 Item[] 배열 변수 
        this.Items.items = inventoryData.items;
    }

    public InventoryData SaveData()
    {
        InventoryData data = new InventoryData();

        data.capacity = capacity;
        data.coins = coins;
        data.items = new Item[capacity];
        for (int i = 0; i < capacity; i++)
        {
            data.items[i] = this.Items[i];
        }

        return data;
    }


    public void AddCoins(int amount) => coins += amount;

    public void MinusCoins(int amount) => coins -= amount;

    public bool EnoughCoins(int price) => coins >= price;

    public Item Get(int index) => this.Items[index];

    public void Clear() => this.Items.Clear();

    public bool Add(Item item) => this.Items.TryAdd(item);

    public bool Remove(Item item) => this.Items.TryRemove(item); 

    public void Swap(int src, int dst) => this.Items.Swap(src, dst);

    public int Contains(Item item) => this.Items.Contains(item);

    public bool PlusQuantity(int index, Item item, int quantity = 1) => this.Items.TryPlusQuantity(index, item, quantity);

    public bool MinusQuantity(int index, Item item, int quantity = 1)  => this.Items.TryMinusQuantity(index, item, quantity);

    public int Quantity(Item item) => this.Items.Quantity(item);

    public int Combine(int src, int dst)
    {
        int total = Items[src].quantity + Items[dst].quantity;
        Items[dst].quantity = total;

        Remove(Items[src]);

        return total;
    }

}
