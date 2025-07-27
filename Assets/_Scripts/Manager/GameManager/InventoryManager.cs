using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    InventoryView inventoryView() => inventory.GetInventoryView();
    Inventory inventory;
    InventoryController controller() => inventory.GetInventoryController();

    // TODO : 할당할 방법 찾아야 한다 
    // 테스트 시 인스펙터 할당 
    [SerializeField] Player player;

    private void Awake()
    {
        // this method must be excuted in INGAME
        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            SetupInGameScene();
        }
        
        SetupInGameScene();
    }

    public void SetupInGameScene()
    {
        inventory = GameObject.Find("Item Inventory").GetComponent<Inventory>();
        player = GameObject.Find("Player").GetComponent<Player>();


        Debug.Assert(inventory != null, "item manager inventory null");
        Debug.Assert(player != null, "item manager player null!");
        Debug.Assert(controller() != null, "item manager controller null");
    }

    public void AcquireItem(Item item)
    {
        controller().AcquireItem(item);
    }

    public void PurchaseItem(Item item)
    {
        controller().PurchaseItem(item);
    }

    public void Usetem(Item item)
    {
        if (controller().UseItem(item))
        {
            item.details.UseItem(player);
        }
    }

    public bool UseItem(int index)
    {
        bool use = controller().UseItem(index, player);
        return use;
    }

    public void SwapItems(int index1, int index2)
    {
        controller().SwapItems(index1, index2);
    }

    public void UpdateCoinAmount(int amount)
    {
        controller().UpdateCoins(amount);
    }


    public async void CheckAbandonItem(DraggableItem requester, Item item)
    {
        bool abandon = await inventoryView().CheckAbandonItem(item);

        if (abandon)
        {
            requester.ConfirmAbandonItem();
            controller().AbandonItem(item);
        }
        else
        {
            requester.CancelAbandonItem();
        }
    }

}
