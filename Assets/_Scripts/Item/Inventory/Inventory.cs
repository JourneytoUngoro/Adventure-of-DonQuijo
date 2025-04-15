using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 씬 오브젝트에 붙어있어야 한다
public class Inventory : MonoBehaviour, IDataPersistance
{
    [field : SerializeField] public int capacity { get; private set; }
    [field : SerializeField] public List<ItemDetails> startingItems { get;  set; } // 테스트 용도 

    InventoryView view;
    InventoryController controller;

    public const int InventorySlotsCount = 3; // 세 개로 고정 

    private void Awake()
    {
        view = GetComponent<InventoryView>();

        // view, startingItems를 이용하여 InventoryModel을 생성한 뒤,
        // InventoryController에게 view와 model을 할당한다 
        controller = new InventoryController.Builder(view)
            .WithStartingItems(startingItems)
            .WithCapacity(InventorySlotsCount)
            .Build();
    }

    public void LoadData(GameData data)
    {
        // Save-Load System에서 호출된다
        // TODO : Load 로직 작성하기

        // controller.LoadData();
     }

    public void SaveData(GameData data)
    {
        // Save-Load System에서 호출된다
        // TODO Save 로직 작성하기
    }

    public InventoryController GetInventoryController()
    {
        return controller;
    }


}