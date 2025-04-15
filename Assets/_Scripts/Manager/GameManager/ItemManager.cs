using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemManager : MonoBehaviour
{

    Inventory inventory;
    InventoryController controller() => inventory.GetInventoryController();

    // TODO : 할당할 방법 찾아야 한다 
    // 테스트 시 인스펙터 할당 
    [SerializeField] Player player;


    private void Awake()
    {
        inventory = GameObject.Find("Item Inventory").GetComponent<Inventory>();
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


}
