using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemDatabase
{
    static Dictionary<int, ItemDetails> itemDetailsDictionary;

    public static int totalItems;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void Initialize()
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();

        var itemDetails = Resources.LoadAll<ItemDetails>("Item/ItemDetails");
        foreach (var details in itemDetails)
        {
            itemDetailsDictionary.Add(details.id, details);
        }
        totalItems = itemDetailsDictionary.Count;
    }

    public static ItemDetails GetDetailsById(int id)
    {
        try
        {
            // Debug.Log(itemDetailsDictionary[id] == null ? "DB에 존재하지 않는 아이템" : "DB에 존재하는 아이템");
            return itemDetailsDictionary[id];
        }
        catch
        {
            Debug.LogError($"Cannot find details details : {id}");
            return null;
        }
    }



}
