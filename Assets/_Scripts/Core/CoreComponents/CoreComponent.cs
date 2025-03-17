using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreComponent : MonoBehaviour
{
    protected Entity entity;
    protected float epsilon = 0.001f;
    protected Vector2 v2WorkSpace;
    protected Vector3 v3WorkSpace;

    protected virtual void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }
}