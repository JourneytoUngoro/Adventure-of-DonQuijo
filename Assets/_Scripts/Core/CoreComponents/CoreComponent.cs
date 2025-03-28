using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreComponent : MonoBehaviour
{
    protected Entity entity;
    protected Vector3 workSpace;
    protected const float epsilon = 0.01f;

    protected virtual void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }
}