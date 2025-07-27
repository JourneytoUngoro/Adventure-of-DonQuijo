using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<SceneField> excludedSceneInventoryUI;
    [SerializeField] private Inventory _inventory; // This script is attached to the  'Inventory > Item Inventory object' in the scene
    private Inventory Inventory
    {
        get
        {
            if (_inventory == null)
                _inventory = GameObject.Find("Item Inventory").GetComponent<Inventory>();
            return _inventory;
        }
    }

    private Player _player;
    private Player Player
    {
        get
        {
            if (_player == null)
                _player = GameObject.Find("Player").GetComponent<Player>();
            return _player;
        }
    }

    private InventoryView inventoryView() => Inventory.GetInventoryView();
    private InventoryController controller() => Inventory.GetInventoryController();

    private GameObject inventoryUI;

    private void Awake()
    {
        InitializeInventory();
    }

    private void InitializeInventory()
    {
        inventoryUI = GameObject.Find("Inventory");
        Inventory.Initialize();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($" {gameObject.name} : {scene.name}");
    }

    //private void OnEnable()
    //{
    //    SceneManager.sceneLoaded += OnSceneLoaded;
    //}

    //private void OnDisable()
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}

    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    Debug.Log("======================새씬====================="); // ================================================= 얘 왜 실행 안 되는지? 다른 애들 다 되는데
    //    //if (IsExcludedScene(scene.name))
    //    //{
    //    //    // 인벤토리 off
    //    //    Debug.Log("꺼짐");
    //    //    inventoryUI.SetActive(false);
    //    //}
    //    //else
    //    //{
    //    //    // 인벤토리 on 
    //    //    Debug.Log("켜짐");
    //    //    inventoryUI.SetActive(true);
    //    //    inventoryUI.GetComponent<CanvasGroup>().alpha = 1.0f;
    //    //}
    //}

    //private bool IsExcludedScene(string currentScene)
    //{
    //    return excludedSceneInventoryUI.Any(sceneField => sceneField.SceneName == currentScene);
    //}

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
            item.details.UseItem(Player);
        }
    }

    public bool UseItem(int index)
    {
        bool use = controller().UseItem(index, Player);
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
