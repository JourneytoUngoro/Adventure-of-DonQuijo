using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemManager : MonoBehaviour
{
    private static ItemManager instance;

    //TODO : Manager 통합 전까지는 임시로 이용한다 
    public static ItemManager Instance { get => instance; set => instance = value; }

    public Inventory inventory;

    private InventoryController controller() => inventory.GetInventoryController();

    // TODO : 할당할 방법 찾아야 한다 
    // 테스트 시 인스펙터 할당 
    [SerializeField] Player player;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this); return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);

        inventory = GetComponent<Inventory>();

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

    public void UseItem(int index)
    {
        controller().UseItem(index, player);
    }

    public void SwapItems(int index1, int index2)
    {
        controller().SwapItems(index1, index2);
    }


}
