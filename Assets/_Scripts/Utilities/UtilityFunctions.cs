using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = System.Random;

public static class UtilityFunctions
{
    private static Random random = new Random();

    public static T GetRandom<T>(this IEnumerable<T> objects) => objects.ElementAt(random.Next(objects.Count()));
    public static bool Empty<T>(this IEnumerable<T> objects) => objects.Count() == 0 || objects.All(element => element == null);
    public static float RandomFloat(float minValue, float maxValue) => (float)random.NextDouble() * (maxValue - minValue) + minValue;
    public static int RandomInteger(int minValue, int maxValue) => random.Next(minValue, maxValue);
    public static int RandomInteger(int maxValue) => random.Next(maxValue);
    public static bool IsInLayerMask(this GameObject obj, LayerMask mask) => (mask.value & (1 << obj.layer)) != 0;
    public static bool IsInLayerMask(int layer, LayerMask mask) => (mask.value & (1 << layer)) != 0;
    public static bool isEqual(float a, float b) => Mathf.Abs(a - b) < float.Epsilon;
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

    public static Vector3 ClosestPointOnLine(this Vector2 basePosition, Vector2 lineStart, Vector2 lineEnd, bool infinite = true)
    {
        Vector2 lineDirection = (lineEnd - lineStart);
        float len = lineDirection.magnitude;
        lineDirection.Normalize();

        Vector2 projection = basePosition - lineStart;
        float distance = Vector3.Dot(projection, lineDirection);
        distance = infinite ? distance : Mathf.Clamp(distance, 0f, len);

        return lineStart + lineDirection * distance;
    }

    public static Vector3? GetIntersection(this Vector3 startPosition, Vector3 direction, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 baseLineStart = startPosition;
        Vector3 baseLineEnd = startPosition + direction;

        float denominator = (lineEnd.y - lineStart.y) * (baseLineEnd.x - baseLineStart.x) - (lineEnd.x - lineStart.x) * (baseLineEnd.y - baseLineStart.y);

        if (denominator == 0) return null;

        float u_a = ((lineEnd.x - lineStart.x) * (baseLineStart.y - lineStart.y) - (lineEnd.y - lineStart.y) * (baseLineStart.x - lineStart.x)) / denominator;
        float u_b = ((baseLineEnd.x - baseLineStart.x) * (baseLineStart.y - lineStart.y) - (baseLineEnd.y - baseLineStart.y) * (baseLineStart.x - lineStart.x)) / denominator;

        if (u_b >= 0 && u_b <= 1)
        {
            float intersectionX = baseLineStart.x + u_a * (baseLineEnd.x - baseLineStart.x);
            float intersectionY = baseLineStart.y + u_a * (baseLineEnd.y - baseLineStart.y);

            return new Vector3(intersectionX, intersectionY);
        }

        return null;
    }

    public static bool CompleteOverlap(this BoxCollider2D overlappedCollider, Collider2D overlappingCollider)
    {
        List<Vector3> edgePoints = new List<Vector3>();

        for (int xPos = -1; xPos <= 1; xPos++)
        {
            for (int yPos = -1; yPos <= 1; yPos++)
            {
                edgePoints.Add(overlappedCollider.bounds.center + Vector3.right * xPos * overlappedCollider.bounds.extents.x + Vector3.up * yPos * overlappedCollider.bounds.extents.y);
            }
        }
        
        foreach (Vector3 edgePoint in edgePoints)
        {
            if (!overlappingCollider.OverlapPoint(edgePoint))
            {
                return false;
            }
        }

        return true;
    }
}
