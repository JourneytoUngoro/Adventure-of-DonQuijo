using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController
{
    InventoryView view;
    InventoryModel model;
    int capacity;

    [ShowInInspector] public Item[] itemUsageData;

    InventoryController(InventoryView view, in InventoryModel model, int capacity)
    {
        this.view = view;
        this.model = model;
        this.capacity = capacity;

        // 코루틴 실행을 view에게 위임 
        view.StartCoroutine(Initialize());
    }

    public void LoadInventoryData(InventoryData data)
    {
        model.LoadData(data);
        RefreshCoins();
    }

    public InventoryData SaveInventoryData() => model.SaveData();

    public void LoadItemUsageData(ItemUsageData data)
    {
        this.itemUsageData = data.itemUsageData;

        for (int i = 0; i < itemUsageData.Length; i++)
        {
            if (itemUsageData[i].details == null)
            {
                itemUsageData[i].details = ItemDatabase.GetDetailsById(itemUsageData[i].id);
                Debug.LogWarning($"{itemUsageData[i].details.label} detail allocated again");
            }
        }


        if (this.itemUsageData == null)
        {
            this.itemUsageData = new ItemUsageData().itemUsageData;
        }
    }

    public ItemUsageData SaveItemUsageData()
    {
        ItemUsageData data = new ItemUsageData();
        data.itemUsageData = this.itemUsageData;

        return data;
    }

    IEnumerator Initialize()
    {
        yield return view.InitializeView();

        // view.OnDrop += HandleDrop;
        model.OnModelChanged += HandleModelChanged;
        RefreshView();
        RefreshCoins();
    }

    void HandleModelChanged(Item[] item) => RefreshView();

    public void RefreshView()
    {
        for (int i = 0; i < capacity; i++)
        {
            var item = model.Get(i);

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
                string text = $"인벤토리에 추가됐습니다.";
                SetInventoryTextInfo(text);
                return true;
            }
            else
            {
                // 수량 초과로 추가 실패
                string text = $"최대 보유 수량에 도달했습니다.";
                SetInventoryTextInfo(text);
                return false;
            }
        }
        else
        {
            // 현재 인벤토리에 포함되지 않던 아이템 
            if (model.Add(item))
            {
                // 인벤토리 추가 성공
                string text = $"인벤토리에 추가됐습니다";
                SetInventoryTextInfo(text);
                return true;
            }
            else
            {
                // 인벤토리 용량 초과로 추가 실패
                string text = $"인벤토리가 가득 찼습니다.";
                SetInventoryTextInfo(text);
                return false;

            }
        }
    }

    public void UpdateCoins(int amount)
    {
        if (amount >= 0) { model.AddCoins(amount); }
        else { model.MinusCoins(amount); }
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
            string text = $"코인이 부족합니다.";
            SetInventoryTextInfo(text);
        }

        return false;
    }

    public bool UseItem(Item item, int useQuantity = 1)
    {
        int indexIfExist = model.Contains(item);

        if (indexIfExist == -1) return false;

        // 인벤토리에 아이템 존재
        if (model.Quantity(item) >= useQuantity)
        {
            // 가용 횟수 제한 
            if (!IsUnderMaxOverlap(item))
            {
                // 최대 가용 횟수 초과
                string text = $"{item.details.label} 아이템은 최대 섭취 횟수에 도달했습니다.";
                SetInventoryTextInfo(text);
                return false;
            }

            // 사용 가능 여부
            if (model.MinusQuantity(indexIfExist, item, useQuantity))
            {
                string text = $"{item.details.label} 아이템 섭취에 성공했습니다.";
                SetInventoryTextInfo(text);
                return true;
            }
        } 
        else
        {
            string text = $"{item.details.label} 아이템은 섭취할 수 없습니다.";
            SetInventoryTextInfo(text);
            Debug.LogError($"wrong access to {item.details.label}!");
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

        if (item.details.label == "기억의 조각")
        {
            SetInventoryTextInfo("플레이어 사망 시 자동 섭취되는 아이템입니다.");
           
            return false;
        }

        if (UseItem(item, quantity))
        {
            item.details.UseItem(player);
            return true;
        }
        return false;
    }

    public bool UseItemOnDead(int index, Player player, int quantity = 1)
    {
        // 1번 키 입력 -> 0번 슬롯 사용 
        Item item = model.Get(index - 1);
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

    public void AbandonItem(Item item)
    {
        bool success = model.Remove(item);

        Debug.Log($"Log -1 on successful item abandon, 0 on failure. : {model.Contains(item)}");
    }

    public int GetItemIndex(Item item)
    {
        for (int i = 0; i < capacity; i++)
        {
            if (model.Get(i) == item) return i;
        }
        return -1;
    }

    public int HasItem(string itemLabel)
    {
        int index = -1;
        for (int i = 0; i < capacity; i++)
        {
            Item item = model.Get(i);

            if (item != null && itemLabel.Equals(item.details.label)) { return i + 1; }
        }
        // player doesn't have 
        return  index;
    }

    public bool IsUnderMaxOverlap(Item item)
    {
        for (int i = 0; i < this.itemUsageData.Length; i++)
        {
            if (item.id == this.itemUsageData[i].id)
            {
                if (this.itemUsageData[i].nowUseCount < this.itemUsageData[i].details.maxOverlap)
                {
                    this.itemUsageData[i].nowUseCount++;
                    return true;
                }
            }
        }
        return false;
    }

    public bool CanPurchaseItem(Item item)
    {
        for (int i = 0; i < this.itemUsageData.Length; i++)
        {
            if (item.id == this.itemUsageData[i].id)
            {
                if (this.itemUsageData[i].nowUseCount < this.itemUsageData[i].details.maxOverlap)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void SwapItems(int index1, int index2)
    {
        model.Swap(index1-1, index2-1);
    }

    private void SetInventoryTextInfo(string info)
    {
        TextInfoUI textInfo = Manager.Instance.uiManager.ShowDynamicTextInfo(new TextInfoData(info));
        textInfo.SetAnchoredPosition(0, -400);
        textInfo.ShowAndHideUI(1.8f);
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
