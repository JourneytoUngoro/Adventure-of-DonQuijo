using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = System.Random;

public static class UtilityFunctions
{
    private static Random random = new Random();

    public static T2 GetRandom<T1, T2>(T1 objects) where T1 : IEnumerable<T2> => objects.ElementAt(random.Next(objects.Count()));
    public static T GetRandom<T>(this IEnumerable<T> objects) => objects.ElementAt(random.Next(objects.Count()));
    public static float RandomFloat(float minValue, float maxValue) => (float)random.NextDouble() * (maxValue - minValue) + minValue;
    public static int RandomInteger(int minValue, int maxValue) => random.Next(minValue, maxValue);
    public static int RandomInteger(int maxValue) => random.Next(maxValue);
    public static bool IsInLayerMask(this GameObject obj, LayerMask mask) => (mask.value & (1 << obj.layer)) != 0;
    public static bool IsInLayerMask(int layer, LayerMask mask) => (mask.value & (1 << layer)) != 0;
    public static int RandomOption(this IEnumerable<float> possibilities)
    {
        int totalOption = possibilities.Count();
        float totalPossibility = possibilities.Sum();
        float randomFloat = RandomFloat(0, totalPossibility);

        float currentChance = 0.0f;
        for (int option = 0; option < totalOption; option++)
        {
            currentChance += possibilities.ElementAt(option);
            if (randomFloat <= currentChance)
            {
                return option;
            }
        }

        return totalOption - 1;
    }

    public static IList<T> Shuffle<T>(this IList<T> values, bool overwrite)
    {
        IList<T> list = overwrite ? values : values.ToList();

        for (int i = list.Count - 1; i > 0; i--)
        {
            int k = random.Next(i + 1);
            T value = list[k];
            list[k] = list[i];
            list[i] = value;
        }

        return list;
    }

    public static T[] GetComponentsInChildren<T>(this GameObject gameObject, bool includeInactive, bool includeParent = true) where T : Component
    {
        T[] components = gameObject.GetComponentsInChildren<T>(includeInactive);

        if (!includeParent)
        {
            return components.Where(component => component.gameObject != gameObject).ToArray();
        }
        else
        {
            return components.ToArray();
        }
    }

    public static T GetComponentInChildren<T>(this GameObject gameObject, bool includeInactive, bool includeParent = true) where T : Component
    {
        T[] components = gameObject.GetComponentsInChildren<T>(includeInactive);

        if (!includeParent)
        {
            return components.Where(component => component.gameObject != gameObject).FirstOrDefault();
        }
        else
        {
            return components.FirstOrDefault();
        }
    }

    public static GameObject[] FindGameObjectsByLayer(LayerMask layerMask, FindObjectsSortMode objectsSortMode)
    {
        GameObject[] gameObjects = GameObject.FindObjectsByType(typeof(GameObject), objectsSortMode) as GameObject[];
        List<GameObject> returnValue = new List<GameObject>();
        
        for (int i = 0; i < gameObjects.Length; i++)
        { 
            if (gameObjects[i].IsInLayerMask(layerMask))
            {
                returnValue.Add(gameObjects[i]);
            }
        }
        
        if (returnValue.Count == 0)
        {
            return null;
        }
        return returnValue.ToArray();
    }
}
