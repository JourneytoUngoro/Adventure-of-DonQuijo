using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class InventoryController
{
    InventoryView view;
    InventoryModel model;
    int capacity;

    InventoryController(InventoryView view, in InventoryModel model, int capacity)
    {
        Debug.Assert(view != null, "View is null");
        Debug.Assert(model != null, "Model is null");
        Debug.Assert(capacity > 0, "Capacity is less than 1");

        this.view = view;
        this.model = model;
        this.capacity = capacity;

        // 코루틴 실행을 view에게 위임 
        view.StartCoroutine(Initialize());
    }

    public void LoadData(InventoryData data) => model.LoadData(data);
    public InventoryData SaveData() => model.SaveData();

    IEnumerator Initialize()
    {
        yield return view.InitializeView();

        // view.OnDrop += HandleDrop;
        model.OnModelChanged += HandleModelChanged;

        RefreshView();
        RefreshCoins();
    }

    void HandleModelChanged(Item[] item) => RefreshView();

    void RefreshView()
    {
        for (int i = 0; i < capacity; i++)
        {
            var item = model.Get(i);

            Debug.Log(item != null ? $"{i}번째 슬롯 아이템 {item.details.label}" : $"{i} 번째 슬롯 비어있음");

            view.RefreshSlots(i, item);
        }
    }

    public void RefreshCoins()
    {
        view.RefreshCoins(model.coins);
    }

    public bool AcquireItem(Item item, int quantity = 1)
    {
        int indexIfExist = model.Contains(item);

        if (indexIfExist != -1)
        {
            // 현재 인벤토리에 포함된 아이템
            if (model.PlusQuantity(indexIfExist, item, quantity))
            {
                // 추가 성공
                Manager.Instance.uiManager.ShowDynamicTextInfo(new TextInfoData(
                    $"Success Acquire Item {item.details.label}, now quantity {model.Quantity(item)} ")).ShowAndHideUI(3f);
                return true;
            }
            else
            {
                // 수량 초과로 추가 실패
                Manager.Instance.uiManager.ShowDynamicTextInfo(new TextInfoData(
                    $"Fail Acquire Item {item.details.label}, now quantity {model.Quantity(item)} > max quantity {item.details.maxStack} ")).ShowAndHideUI(3f);
                return false;
            }
        }
        else
        {
            // 현재 인벤토리에 포함되지 않던 아이템 
            if (model.Add(item))
            {
                // 인벤토리 추가 성공
                Manager.Instance.uiManager.ShowDynamicTextInfo(new TextInfoData(
                    $"Success Acquire Item {item.details.label}, now index {model.Contains(item)}, now quantity {model.Quantity(item)} ")).ShowAndHideUI(3f);
                return true;
            }
            else
            {
                // 인벤토리 용량 초과로 추가 실패
                Manager.Instance.uiManager.ShowDynamicTextInfo(new TextInfoData(
                    $"Fail Acquire Item {item.details.label}, now count {model.Items.Count}")).ShowAndHideUI(3f);
                return false;

            }

        }

    }

    public void AddCoins(int amount)
    {
        model.AddCoins(amount);
        RefreshCoins();
    }

    public void MinusCoints(int amount)
    {
        model.MinusCoins(amount);
        RefreshCoins();
    }

    public bool PurchaseItem(Item item)
    {
        if (model.EnoughCoins(item.details.price))
        {
            // coins 이 충분하고 인벤토리에 남은 슬롯이 있다면
            if (AcquireItem(item))
            {
                model.MinusCoins(item.details.price);
                RefreshCoins();
                return true;
            }
        }
        else
        {
            // coins 이 부족하다
            Manager.Instance.uiManager.ShowDynamicTextInfo(new TextInfoData(
                $"Item Price {item.details.price} > Coins {model.coins}")).ShowAndHideUI(3f);
        }

        return false;
    }

    public bool UseItem(Item item, int quantity = 1)
    {
        int indexIfExist = model.Contains(item);

        if (indexIfExist != -1)
        {
            // 인벤토리에 아이템 존재
            if (model.MinusQuantity(indexIfExist, item, quantity))
            {
                // 수량 감소 성공
                Manager.Instance.uiManager.ShowDynamicTextInfo(new TextInfoData(
                    $"Success Use Item {item.details.label}, now quantity {model.Quantity(item)}")).ShowAndHideUI(3f);
                return true;
            } 
            else
            {
                Manager.Instance.uiManager.ShowDynamicTextInfo(new TextInfoData(
                    $"Fail Use Item {item.details.label}, now quantity {model.Quantity(item)}")).ShowAndHideUI(3f);
            }
        }
        return false;
    }

    public bool UseItem(int index, Player player, int quantity = 1)
    {
        // 1번 키 입력 -> 0번 슬롯 사용 
        Item item = model.Get(index-1);
        if (item == null)
        {
            // 비어있는 슬롯 사용
            Debug.Log("비어있는 인벤토리를 사용했습니다.");
            return false;
        }
        if (UseItem(item, quantity))
        {
            item.details.UseItem(player);
            return true;
        }
        return false;
    }

    public void SwapItems(int index1, int index2)
    {
        model.Swap(index1-1, index2-1);
    }





    #region Builder : 컨트롤러 객체를 조건대로 생성하여 반환하는 클래스

    public class Builder
    {
        InventoryView view;
        IEnumerable<ItemDetails> itemDetails;
        int capacity;

        public Builder(InventoryView view)
        {
            this.view = view;
        }

        public Builder WithStartingItems(IEnumerable<ItemDetails> itemDetails)
        {
            this.itemDetails = itemDetails;
            return this;
        }

        public Builder WithCapacity(in int capacity)
        {
            this.capacity = capacity;
            return this;
        }

        public InventoryController Build()
        {
            InventoryModel model = itemDetails != null
                ? new InventoryModel(itemDetails, capacity)
                : new InventoryModel(Array.Empty<ItemDetails>(), capacity);

            return new InventoryController(view, model, capacity);
        }



        #endregion


    }
}
